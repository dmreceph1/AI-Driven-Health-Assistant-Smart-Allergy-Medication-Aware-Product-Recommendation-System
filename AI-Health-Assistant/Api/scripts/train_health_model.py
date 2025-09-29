import tensorflow as tf
import numpy as np
from sklearn.model_selection import train_test_split
import pandas as pd

# Örnek veri oluştur (gerçek uygulamada veritabanından gelecek)
def generate_sample_data(num_samples=1000):
    data = []
    for _ in range(num_samples):
        height = np.random.normal(170, 10)  # cm
        weight = np.random.normal(70, 15)   # kg
        bmi = weight / ((height/100) ** 2)
        allergy_count = np.random.randint(0, 5)
        medication_count = np.random.randint(0, 5)
        
        # Sağlık skoru hesapla (0-100 arası)
        health_score = 100
        if bmi < 18.5 or bmi > 25:
            health_score -= 20
        health_score -= allergy_count * 5
        health_score -= medication_count * 3
        health_score = max(0, min(100, health_score))
        
        # Risk faktörleri (0-1 arası)
        cardiovascular_risk = 0.3 if bmi > 25 else 0.1
        metabolic_risk = 0.4 if bmi > 30 else 0.2
        immune_risk = 0.5 if allergy_count > 2 else 0.2
        
        # Öneriler (0-1 arası)
        cardio_recommendation = 0.8 if bmi > 25 else 0.3
        nutrition_recommendation = 0.9 if bmi < 18.5 or bmi > 25 else 0.4
        stress_recommendation = 0.7 if medication_count > 2 else 0.3
        sleep_recommendation = 0.6 if health_score < 70 else 0.3
        
        data.append([
            height, weight, bmi, allergy_count, medication_count,
            health_score/100,
            cardiovascular_risk, metabolic_risk, immune_risk,
            cardio_recommendation, nutrition_recommendation,
            stress_recommendation, sleep_recommendation
        ])
    
    return np.array(data)

# Veri oluştur
data = generate_sample_data()

# Girdi ve çıktı verilerini ayır
X = data[:, :5]  # height, weight, bmi, allergy_count, medication_count
y = data[:, 5:]  # health_score, risk_factors, recommendations

# Veriyi normalize et
X = X / np.array([200, 150, 40, 10, 10])  # Maksimum değerlerle normalize et

# Eğitim ve test verilerini ayır
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Model oluştur
model = tf.keras.Sequential([
    tf.keras.layers.Dense(64, activation='relu', input_shape=(5,)),
    tf.keras.layers.Dropout(0.2),
    tf.keras.layers.Dense(32, activation='relu'),
    tf.keras.layers.Dropout(0.2),
    tf.keras.layers.Dense(8, activation='sigmoid')  # 8 çıktı: health_score + 3 risk + 4 öneri
])

# Modeli derle
model.compile(
    optimizer='adam',
    loss='mse',
    metrics=['mae']
)

# Modeli eğit
model.fit(
    X_train, y_train,
    epochs=50,
    batch_size=32,
    validation_split=0.2,
    verbose=1
)

# Modeli değerlendir
test_loss, test_mae = model.evaluate(X_test, y_test, verbose=0)
print(f'Test Loss: {test_loss:.4f}')
print(f'Test MAE: {test_mae:.4f}')

# TensorFlow Lite modeline dönüştür
converter = tf.lite.TFLiteConverter.from_keras_model(model)
tflite_model = converter.convert()

# Modeli kaydet
with open('health_model.tflite', 'wb') as f:
    f.write(tflite_model)

print('Model başarıyla eğitildi ve kaydedildi.') 