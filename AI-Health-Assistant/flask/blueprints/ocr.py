from flask import Blueprint, request, jsonify
from PIL import Image
import numpy as np
import pytesseract 
import cv2 
from pyzbar.pyzbar import decode
import io

ocr_bp = Blueprint('ocr', __name__)

pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

def find_text_regions_and_recognize(img_np):
    print("...Kontur tabanlı metin tespiti işlemi başlıyor...") 
    gray = cv2.cvtColor(img_np, cv2.COLOR_BGR2GRAY)

    try:
        thresh_val, binary_img = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)
        print(f"Otsu eşik değeri: {thresh_val}") # Debug mesajı
    except Exception as e:
        print(f"Eşikleme hatası: {e}. Standart eşikleme denenecek.")
      
        thresh_val, binary_img = cv2.threshold(gray, 127, 255, cv2.THRESH_BINARY_INV)

    kernel_size = 2 
    kernel = np.ones((kernel_size, kernel_size), np.uint8)
    binary_img = cv2.morphologyEx(binary_img, cv2.MORPH_OPEN, kernel, iterations=1)
  
    contours, hierarchy = cv2.findContours(binary_img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    print(f"Toplam {len(contours)} adet dış kontur bulundu.") 

    detected_text_parts = []

    min_area = 10      
    max_area = 20000     
    min_height = 5     
    max_height = 300   
    min_aspect_ratio = 0.05 
    max_aspect_ratio = 10.0 

    img_with_contours = img_np.copy()

    potential_char_count = 0
    for contour in contours:
        x, y, w, h = cv2.boundingRect(contour)

        area = w * h 
        aspect_ratio = w / float(h) if h > 0 else 0

        if min_area < area < max_area and \
           min_height < h < max_height and \
           min_aspect_ratio < aspect_ratio < max_aspect_ratio:

            potential_char_count += 1
            padding = 10
            roi_y_start = max(0, y - padding)
            roi_y_end = min(gray.shape[0], y + h + padding)
            roi_x_start = max(0, x - padding)
            roi_x_end = min(gray.shape[1], x + w + padding)

            roi = gray[roi_y_start:roi_y_end, roi_x_start:roi_x_end]

            if roi.size == 0: 
                continue

            custom_config = r'--oem 3 --psm 6 -l tur+eng'
            text = pytesseract.image_to_string(gray, config=custom_config)
            print("Tüm görselde OCR sonucu:", text)
            try:     
                char_text = pytesseract.image_to_string(roi, config=custom_config)
                cleaned_text = ''.join(filter(str.isalnum, char_text)).strip()

                if cleaned_text:
                    detected_text_parts.append({'text': cleaned_text, 'x': x, 'y': y})
                    cv2.rectangle(img_with_contours, (x, y), (x + w, y + h), (0, 255, 0), 2)
               
            except pytesseract.TesseractNotFoundError:
                 print("HATA: Tesseract bulunamadı veya yolu yanlış.")
                 raise 
            except Exception as e:
                print(f"ROI üzerinde Tesseract hatası (x={x}, y={y}): {e}")
                cv2.rectangle(img_with_contours, (x, y), (x + w, y + h), (255, 0, 0), 1)
                pass 

    print(f"Filtrelemeden sonra {potential_char_count} potansiyel karakter/bölge bulundu.")
    print(f"Tanınan metin parçası sayısı: {len(detected_text_parts)}")

    detected_text_parts.sort(key=lambda item: item['x'])

    full_text = ' '.join([part['text'] for part in detected_text_parts if part['text'][0].isupper()])

    print(f"Birleştirilmiş Metin: {full_text}") # Debug mesajı
    return full_text.strip()

@ocr_bp.route('/analyze', methods=['POST'])
def analyze_image():
    if 'image' not in request.files:
        return jsonify({'error': 'Görsel dosyası bulunamadı'}), 400

    image_file = request.files['image']
    mode = request.form.get('mode', '').lower()

    try:
        in_memory_file = io.BytesIO()
        image_file.save(in_memory_file)
        in_memory_file.seek(0) 
        image_data = np.frombuffer(in_memory_file.read(), np.uint8)
        image_np = cv2.imdecode(image_data, cv2.IMREAD_COLOR)
        if image_np is None:
             return jsonify({'error': 'Görüntü dosyası okunamadı veya format desteklenmiyor'}), 400
         
    except Exception as e:
        return jsonify({'error': f'Görüntü işlenirken hata oluştu: {str(e)}'}), 500
    if mode == 'image':
        try:
            text = find_text_regions_and_recognize(image_np)

            if not text:
                 return jsonify({'mode': 'image', 'text': 'Metin tespit edilemedi (kontur yöntemi)'}), 200
            return jsonify({'mode': 'image', 'text': text})

        except pytesseract.TesseractNotFoundError:
             return jsonify({'error': 'Tesseract OCR motoru bulunamadı. Lütfen kurulumu ve yolu kontrol edin.'}), 500
        except Exception as e:
             return jsonify({'error': f'Metin tanıma sırasında hata: {str(e)}'}), 500

    elif mode == 'barcode':
        try:
            barcodes = decode(image_np)

            if not barcodes:
                return jsonify({'mode': 'barcode', 'text': 'Barkod bulunamadı'})

            barcode_texts = [barcode.data.decode('utf-8') for barcode in barcodes]
            return jsonify({'mode': 'barcode', 'text': ', '.join(barcode_texts)})

        except Exception as e:
             return jsonify({'error': f'Barkod okuma sırasında hata: {str(e)}'}), 500

    else:
        return jsonify({'error': 'Geçersiz mod belirtildi. Mod "image" veya "barcode" olmalı.'}), 400 