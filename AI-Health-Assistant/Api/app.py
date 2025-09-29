from flask import Flask, request, jsonify
from PIL import Image
import numpy as np
import cv2 
from pyzbar.pyzbar import decode
import io
import easyocr

app = Flask(__name__)

# EasyOCR okuyucuyu başlat (Türkçe ve İngilizce için)
reader = easyocr.Reader(['tr', 'en'])

def find_text_regions_and_recognize(img_np):
    print("...EasyOCR ile metin tespiti başlıyor...")
    
    try:
        # Görüntüyü yeniden boyutlandır (daha hızlı işlem için)
        height, width = img_np.shape[:2]
        if width > 1000:
            scale = 1000 / width
            img_np = cv2.resize(img_np, None, fx=scale, fy=scale)
        
        # EasyOCR ile metin tespiti
        results = reader.readtext(img_np)
        
        # Tespit edilen metinleri birleştir
        detected_texts = []
        for (bbox, text, prob) in results:
            # Sadece yüksek güvenilirlikli sonuçları al
            if prob > 0.3:  # Güvenilirlik eşiği
                # Metni temizle
                cleaned_text = ''.join(c for c in text if c.isalnum() or c.isspace())
                if len(cleaned_text) >= 2:  # En az 2 karakterli kelimeler
                    detected_texts.append(cleaned_text)
        
        # Metinleri birleştir
        final_text = ' '.join(detected_texts)
        print(f"Tespit edilen metin: {final_text}")
        return final_text.strip()
        
    except Exception as e:
        print(f"EasyOCR hatası: {e}")
        return ""

@app.route('/analyze', methods=['POST'])
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
            return jsonify({'error': 'Görüntü dosyası okunamadı'}), 400
         
    except Exception as e:
        return jsonify({'error': f'Görüntü işlenirken hata: {str(e)}'}), 500
        
    if mode == 'image':
        try:
            text = find_text_regions_and_recognize(image_np)
            return jsonify({'mode': 'image', 'text': text})
            
        except Exception as e:
            return jsonify({'error': f'Metin tanıma hatası: {str(e)}'}), 500

    elif mode == 'barcode':
        try:
            barcodes = decode(image_np)
            if not barcodes:
                return jsonify({'mode': 'barcode', 'text': 'Barkod bulunamadı'})
            
            barcode_texts = [barcode.data.decode('utf-8') for barcode in barcodes]
            return jsonify({'mode': 'barcode', 'text': ', '.join(barcode_texts)})
            
        except Exception as e:
            return jsonify({'error': f'Barkod okuma hatası: {str(e)}'}), 500

    else:
        return jsonify({'error': 'Geçersiz mod. "image" veya "barcode" olmalı.'}), 400

if __name__ == '__main__':   
    print("Flask sunucusu başlatılıyor...")
    app.run(host='0.0.0.0', port=5000, debug=True, use_reloader=False) 