from flask import Blueprint, jsonify
import pyodbc
import pandas as pd
from sqlalchemy import create_engine
from mlxtend.preprocessing import TransactionEncoder
from mlxtend.frequent_patterns import apriori, association_rules

veri_madenciligi_bp = Blueprint('veri_madenciligi', __name__)

# SQL Server bağlantı dizesi
conn_str = (
    r'DRIVER={SQL Server};'
    r'SERVER=DESKTOP-QAS58M6\SQLEXPRESS;'
    r'DATABASE=ApiProject;'
    r'Trusted_Connection=yes;'
)
conn_str_sqlalchemy = 'mssql+pyodbc:///?odbc_connect={}'.format(conn_str)
engine = create_engine(conn_str_sqlalchemy)

@veri_madenciligi_bp.route('/stats/gender', methods=['GET'])
def gender_distribution():
    query = """
        SELECT u.Cinsiyet
        FROM [User] u
        LEFT JOIN UserPhysicalInfo upi ON u.UserID = upi.UserID
    """
    df = pd.read_sql(query, engine)
    df['Cinsiyet_Etiket'] = df['Cinsiyet'].map({True: 'Erkek', False: 'Kadın'}).fillna('Bilinmiyor')
    counts = df['Cinsiyet_Etiket'].value_counts().to_dict()
    return jsonify(counts)

@veri_madenciligi_bp.route('/stats/bmi', methods=['GET'])
def bmi_distribution():
    query = """
        SELECT u.UserID, upi.Height, upi.Weight
        FROM [User] u
        LEFT JOIN UserPhysicalInfo upi ON u.UserID = upi.UserID
    """
    df = pd.read_sql(query, engine)
    df.dropna(subset=['Height', 'Weight'], inplace=True)
    df = df[(df['Height'] > 0) & (df['Weight'] > 0)]
    df['BMI'] = df['Weight'] / ((df['Height'] / 100) ** 2)
    bins = pd.cut(df['BMI'], bins=[0, 18.5, 24.9, 29.9, 100], labels=['Zayıf', 'Normal', 'Fazla Kilolu', 'Obez'])
    result = bins.value_counts().sort_index().to_dict()
    return jsonify(result)

@veri_madenciligi_bp.route('/stats/top-allergies', methods=['GET'])
def top_allergies():
    query = """
        SELECT a.AllergyName
        FROM UserAllergy ua
        JOIN Allergy a ON ua.AllergyID = a.AllergyID
    """
    df = pd.read_sql(query, engine)
    top = df['AllergyName'].value_counts().nlargest(5).to_dict()
    return jsonify(top)

@veri_madenciligi_bp.route('/stats/top-medications', methods=['GET'])
def top_medications():
    query = """
        SELECT m.MedicationName
        FROM UserMedication um
        JOIN Medications m ON um.MedicationID = m.MedicationID
    """
    df = pd.read_sql(query, engine)
    top = df['MedicationName'].value_counts().nlargest(5).to_dict()
    return jsonify(top)

@veri_madenciligi_bp.route('/association-rules', methods=['GET'])
def association_rules_api():
    # Alerji ve İlaç verilerini çek
    allergy_query = "SELECT ua.UserID, a.AllergyName FROM UserAllergy ua JOIN Allergy a ON ua.AllergyID = a.AllergyID"
    medication_query = "SELECT um.UserID, m.MedicationName FROM UserMedication um JOIN Medications m ON um.MedicationID = m.MedicationID"
    
    df_allergy = pd.read_sql(allergy_query, engine)
    df_med = pd.read_sql(medication_query, engine)
    
    #çekilen alerji ve ilaç verileri data frame olarak oluşturdum

    # Kullanıcıya ait ilaç ve alerjileri  birleşik liste oluşturur outer adında tek listede birleştirdim
    user_data = pd.merge(
        df_allergy.groupby('UserID')['AllergyName'].apply(list).reset_index(name='Alerjiler'),
        df_med.groupby('UserID')['MedicationName'].apply(list).reset_index(name='İlaçlar'),
        on='UserID', how='outer'
    )

    user_data['Alerjiler'] = user_data['Alerjiler'].apply(lambda x: x if isinstance(x, list) else [])
    user_data['İlaçlar'] = user_data['İlaçlar'].apply(lambda x: x if isinstance(x, list) else [])
    user_data['Tüm_Öğeler'] = user_data.apply(lambda row: row['Alerjiler'] + row['İlaçlar'], axis=1)
    
    #Alerji ve ilaçları birleştirip Tüm_Öğeler adında yeni bir liste kolonu oluşturdum bi nevi sepet gibi

    transactions = user_data[user_data['Tüm_Öğeler'].apply(len) > 0]['Tüm_Öğeler'].tolist()
    if not transactions:
        return jsonify({"error": "Yeterli veri yok"})

    te = TransactionEncoder()
    te_ary = te.fit(transactions).transform(transactions)
    # tüm öğeler listesini one-hot ile 1 ve 0 formatına getirdim
    df_encoded = pd.DataFrame(te_ary, columns=te.columns_)

    # Apriori algoritmasıyla sık öğe kümelerini buluyorum en az %1 oranında geçen alerji/ilaç kombinasyonları
    frequent_itemsets = apriori(df_encoded, min_support=0.01, use_colnames=True)
    if frequent_itemsets.empty:
        return jsonify({"error": "Sık öğe bulunamadı"})
    
    #Sık geçen kümelerden kurallar türetir. lift > 1.1 olacak şekilde
    #lift:birliktelik kuralının ne kadar güçlü olduğu ölçüsü yani örneğin x varken y'nin olma ihtimali gerçekten yüksek mi tesadüf mü 
    rules = association_rules(frequent_itemsets, metric="lift", min_threshold=1.1)
    if rules.empty:
        return jsonify({"error": "Birliktelik kuralı bulunamadı"})
    
    #Lift oranına göre en güçlü ilk 10 kuralı seçiyor.
    rules = rules.sort_values(by='lift', ascending=False)
    top_rules = rules[['antecedents', 'consequents', 'support', 'confidence', 'lift']].head(10)

    # Açıklamalı cümleler oluşturdum
    explanations = []
    for _, row in top_rules.iterrows(): #en güçlü 10 kuralı tek tek dolaşır
        antecedents = list(row['antecedents']) if isinstance(row['antecedents'], frozenset) else []
        #kuralın sol tarafı eğer şunlar varsa ...
        consequents = list(row['consequents']) if isinstance(row['consequents'], frozenset) else []
        #kuralın sağ tarafı ... genellikle bunlarda olur

        if not antecedents or not consequents: #eğer bir taraf boşsa o kuralı atlar
            continue

        explanation = f"{', '.join(antecedents)} olan kullanıcıların, genellikle {', '.join(consequents)} olduğu gözlemlenmiştir."

        explanations.append({
            "antecedents": antecedents, #şartlar
            "consequents": consequents, #sonuçlar
            "support": round(row['support'], 3), #kaç kullanıcıda bu görülmüş
            "confidence": round(row['confidence'], 3), #güven oranı
            "lift": round(row['lift'], 3), #nekadar güçlü ilişki
            "explanation": explanation #türkçe açıklama
        })

    if not explanations:
        return jsonify({"error": "Yorumlanabilir kural bulunamadı"})

    return jsonify(explanations) 