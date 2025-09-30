# 🩺 AI-Powered Health Assistant  
**Yapay Zeka Destekli Sağlık Kontrol ve Ürün Öneri Sistemi**  

Kullanıcıların **ilaç**, **alerji**, **fiziksel bilgiler (boy-kilo)** ve **ürün içerikleri** üzerinden **sağlık analizi** yapabilen, mobil ve web tabanlı çok katmanlı bir sistemdir.  

Bu proje sayesinde kullanıcılar:  
- 📊 Sağlık skorlarını görebilir  
- 🥗 Alışverişte ürün içeriklerini kontrol edebilir  
- ⚠️ Alerji ve ilaç risklerine karşı uyarı alabilir  
- 🤖 Yapay zeka ile kendine uygun benzer ve güvenli ürün önerileri alabilir  
- 📈 Veri madenciliği ve istatistiksel analizlerden yararlanabilir  

---

## 🚀 Özellikler  

### 📱 Mobil (Flutter)  
- Kayıt & giriş (JWT)  
- Profil, alerji ve ilaç yönetimi  
- Görselden **OCR & barkod okuma**  
- Sağlık skoru hesaplama  
- Yapay zeka destekli ürün önerisi  
- İstatistik ve veri madenciliği sonuçlarını **grafiklerle** görselleştirme  

### 💻 Web (ASP.NET MVC)  
- Kullanıcı giriş & profil yönetimi  
- API tüketimi ile ürün sorgulama  
- Sağlık verilerine erişim  

### 🔗 Backend (.NET Core Web API)  
- Repository Pattern + Dapper  
- DTO yapısı  
- JWT ile kimlik doğrulama  
- Kullanıcı, alerji, ilaç, ürün yönetimi  

### 🤖 AI & Veri Analizi (Python Flask)  
- 📷 OCR & barkod analizi (OpenCV + Pyzbar)  
- 🧠 **KNN + TF-IDF + OneHotEncoder** ile ürün öneri sistemi  
- 🧮 Sağlık skoru hesaplama (BMI + alerji + ilaç riski)  
- 📊 Veri madenciliği (**Apriori algoritması** ile birliktelik kuralları)  
- 🔎 İstatistiksel analiz (cinsiyet, BMI, alerji/ilaç dağılımı)  

### 🗄 Veritabanı (SQL Server)  
- Kullanıcı, alerji, ilaç, ürün ve içerik tabloları  
- Güncel/pasif ilaç & alerji takibi (`updateDate`, `inactivateDate`)  
- Kategorisel ürün sınıflandırması (atıştırmalık, içecek, kahvaltılık vs.)  

---

## 📊 Kullanım Senaryosu  
1. Kullanıcı giriş yapar (mobil/web).  
2. Alerji, ilaç ve fiziksel bilgilerini ekler.  
3. Ürün sorgusu yapılır (barkod, isim veya görselden OCR).  
4. Flask AI servisi:  
   - Ürünü analiz eder  
   - Kullanıcının alerji & ilaç risklerini kontrol eder  
   - Sağlık skoru hesaplar  
   - Benzer/güvenli ürün önerileri sunar  
5. Mobil uygulama verileri grafiklerle görselleştirir  

---

## 📈 Veri Madenciliği & Yapay Zeka  
- **Apriori algoritması** → Alerji & ilaç birliktelik kuralları (lift, confidence)  
- **KNN algoritması** → Kullanıcıya uygun en yakın ürünleri önerme  
- **TF-IDF** → Ürün adları arasında benzerlik analizi  
- **Sağlık Skoru** → BMI + alerji + ilaç faktörleri ile 0–100 arası puan  

---

## 🔮 Gelecek Çalışmalar  
- Günlük **kişisel sağlık takvimi**  
- Yapay zeka destekli **to-do list** + motivasyon mesajları  
- Daha gelişmiş **derin öğrenme tabanlı OCR**  
- Cloud entegrasyonu (Azure / AWS)  

---
<img width="1582" height="723" alt="web" src="https://github.com/user-attachments/assets/0bc4c21f-2ec0-42cf-a779-2ed8ad15a730" />
<p align="center">
  <img src="https://github.com/user-attachments/assets/92c51bb3-49ed-42a3-b6e8-81ea76c04b28" alt="mob1" width="150" style="margin-right:20px;">
  <img src="https://github.com/user-attachments/assets/14ace470-c084-40a7-bad8-06f39d83123b" alt="mob2" width="150">
</p>
---
https://youtu.be/GwgBSEXAsdQ

👨‍💻 **Geliştirici:** Recep Demir  
🎓 **Kayseri Üniversitesi – Bilgisayar Mühendisliği Bitirme Projesi (2025)**  
