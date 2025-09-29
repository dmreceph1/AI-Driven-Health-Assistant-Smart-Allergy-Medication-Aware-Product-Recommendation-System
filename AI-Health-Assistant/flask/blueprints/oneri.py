from flask import Blueprint, request, jsonify
import pyodbc
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import OneHotEncoder
from sklearn.neighbors import NearestNeighbors
import numpy as np

oneri_bp = Blueprint('oneri', __name__)

conn = pyodbc.connect(
    'DRIVER={SQL Server};'
    'SERVER=DESKTOP-QAS58M6\\SQLEXPRESS;'
    'DATABASE=ApiProject;'
    'Trusted_Connection=yes;'
)

def fetch_data():
    query = """
    SELECT p.ProductID, p.ProductName, p.Price, p.CategoryID, 
           p.IsSalty, p.IsSugary, p.IsSpicy, p.IsHealthy, p.Calories,
           c.CategoryName
    FROM Product p
    JOIN Category c ON p.CategoryID = c.CategoryID
    """
    return pd.read_sql(query, conn)

def prepare_knn_features(product_df):
    tfidf = TfidfVectorizer()
    product_df['ProductName'] = product_df['ProductName'].fillna('')
    tfidf_matrix = tfidf.fit_transform(product_df['ProductName']) # ürün adını sayısal vektöre dönüştürdüm

    encoder = OneHotEncoder(sparse_output=False, handle_unknown='ignore') 
    product_df['CategoryName'] = product_df['CategoryName'].fillna('')
    encoded_categories = encoder.fit_transform(product_df[['CategoryName']]) 
    # her kategori one-hot ile binary hale çevrildi
    numeric_features = product_df[['Price']].fillna(0).values * 2.0 #fiyat sağlıklı olması ve kalori knn de daha belirleyici 
    nutrition_columns = ['IsSalty', 'IsSugary', 'IsSpicy', 'IsHealthy', 'Calories']
    nutrition_features = product_df[nutrition_columns].fillna(0).values
    nutrition_features = nutrition_features * np.array([1.5, 1.5, 1.5, 2.0, 2.0]) 

    final_features = np.hstack([   #Hepsini yatayda birleştirerek tek bir büyük özellik vektörü oluşturdum. her satır ürün her sütun özellik
        tfidf_matrix.toarray(),
        encoded_categories,
        numeric_features,
        nutrition_features
    ])

    return final_features, tfidf, encoder 

def fetch_user_allergy_and_medications(userid):
    allergy_query = """
    SELECT a.MedicationContent
    FROM UserAllergy ua
    JOIN Allergy a ON ua.AllergyID = a.AllergyID
    WHERE ua.UserID = ? AND ua.UpdateDate IS NULL
    """
    allergy_df = pd.read_sql(allergy_query, conn, params=[userid])

    medication_query = """
    SELECT m.ContraindicatedContent
    FROM UserMedication um
    JOIN Medications m ON um.MedicationID = m.MedicationID
    WHERE um.UserID = ? AND um.InactiveDate IS NULL
    """
    medication_df = pd.read_sql(medication_query, conn, params=[userid])

    allergy_contents = []
    for _, row in allergy_df.iterrows():
        if row['MedicationContent'] and pd.notna(row['MedicationContent']):
            allergy_contents.extend([x.strip().lower() for x in str(row['MedicationContent']).split(',')])

    medication_contents = []
    for _, row in medication_df.iterrows():
        if row['ContraindicatedContent'] and pd.notna(row['ContraindicatedContent']):
            medication_contents.extend([x.strip().lower() for x in str(row['ContraindicatedContent']).split(',')])

    print(f" Kullanıcının alerji içerikleri: {allergy_contents}")
    print(f" Kullanıcının ilaç içerikleri: {medication_contents}")

    return set(allergy_contents), set(medication_contents)

@oneri_bp.route('/filterProducts', methods=['POST'])
def filter_products():
    try:
        data = request.get_json()
        contents_from_ocr = data.get('contents', [])
        if contents_from_ocr is None:
            contents_from_ocr = []
        
        product_id = data.get('productID')
        userid = data.get('userId')

        if not contents_from_ocr and not product_id and not userid: 
             return jsonify({'message': 'productID ve userId zorunludur ancak contents boş olabilir.'}), 400
        if not product_id or not userid:
            return jsonify({'message': 'productID ve userId alanları zorunludur!'}), 400


        print(f" Gelen içerikler: {contents_from_ocr}")

        product_df = fetch_data()
        if product_df.empty:
            return jsonify({'message': 'Veritabanından ürün verisi alınamadı.'}), 500

        if not product_df[product_df['ProductID'] == product_id].empty:
            original_idx = product_df[product_df['ProductID'] == product_id].index[0]
        else:
            return jsonify({'message': f'Girilen ProductID ({product_id}) veritabanında bulunamadı.'}), 404
            
        final_features, _, _ = prepare_knn_features(product_df.copy()) 

        allergy_set, medication_set = fetch_user_allergy_and_medications(userid)

        processed_ocr_contents = set([c.strip().lower() for c in contents_from_ocr if isinstance(c, str)])

        all_risk_contents = processed_ocr_contents | allergy_set | medication_set
        all_risk_contents = {risk for risk in all_risk_contents if risk} 
        print(f" Birleşmiş riskli içerikler: {all_risk_contents}")

        cursor = conn.cursor()
        cursor.execute("SELECT CategoryID FROM Product WHERE ProductID = ?", (product_id,))
        row = cursor.fetchone()
        if not row:
            return jsonify({'message': 'Ürün bulunamadı'}), 404
        category_id = row[0]

        cursor.execute("""
            SELECT p.ProductID, pc.Component
            FROM Product p
            LEFT JOIN ProductContents pc ON p.ProductID = pc.ProductID
            WHERE p.CategoryID = ? AND p.ProductID != ?
        """, (category_id, product_id))
        
        product_contents_map = {}
        for p_id_map, component in cursor.fetchall():
            if p_id_map not in product_contents_map:
                product_contents_map[p_id_map] = set()
            if component and pd.notna(component): 
                product_contents_map[p_id_map].add(str(component).strip().lower())

        safe_ids = []
        for p_id_map, current_product_ingredients_set in product_contents_map.items():
            product_is_unsafe = False
            if not all_risk_contents:
                pass 
            elif not current_product_ingredients_set and all_risk_contents: 
                 pass 
            else:
                for single_product_ingredient_string in current_product_ingredients_set:
                    if not single_product_ingredient_string:
                        continue
                    for user_risk_item in all_risk_contents:
                        if user_risk_item in single_product_ingredient_string:
                            product_is_unsafe = True
                            break 
                    if product_is_unsafe:
                        break
            
            if not product_is_unsafe:
                safe_ids.append(p_id_map)

        print(f" Güvenli ürün Id'leri (KNN için adaylar): {safe_ids}")

        if not safe_ids:
            print(" Güvenli ürün bulunamadı.")
            return jsonify([]), 200

        safe_indices = []  #Ürün ID’lerini, product_df tablosundaki index numarasına çevirir.
        for safe_pid in safe_ids:
            idx_list = product_df[product_df['ProductID'] == safe_pid].index.tolist()
            if idx_list: 
                safe_indices.append(idx_list[0])
        
        if not safe_indices:
            print(" KNN için uygun index bulunamadı")
            return jsonify([]), 200
        
        # kullanıcının baktığı ürünün özellik vektörü alınıyor KNN için 2D hale getiriliyor
        original_feature_vector = final_features[original_idx].reshape(1, -1) 
        candidate_features = final_features[safe_indices]
        #Güvenli ürünlerin özellik vektörleri alınıyor bunlar KNN’e verilecek aday ürünler yani

        if candidate_features.shape[0] == 0:
             print(" KNN için aday özellik bulunamadı")
             return jsonify([]), 200

        n_neighbors_knn = min(3, candidate_features.shape[0]) #maks 3 ürün önerilecek
        if n_neighbors_knn == 0: 
            print(" KNN için komşu sayısı 0 öneri yapılamıyor.")
            return jsonify([]), 200
        
        # k komşulu model mesafe ölçüsü olarak öklid
        knn = NearestNeighbors(n_neighbors=n_neighbors_knn, metric='euclidean')
        #güvenli ürünlerde model kullanılacak
        knn.fit(candidate_features)
        #Kullanıcının ürününe en yakın ürünlerin index’leri ve mesafeleri bulunuyor
        distances, indices_in_candidate_features = knn.kneighbors(original_feature_vector)
        
        recommended_ids_from_knn = []
        for i in indices_in_candidate_features[0]:
            recommended_ids_from_knn.append(safe_ids[i])

        final_recommendations = []
        for rec_pid in recommended_ids_from_knn:
            
            product_row = product_df[product_df['ProductID'] == rec_pid]
            if not product_row.empty:
                name = product_row['ProductName'].values[0]
                final_recommendations.append({
                    'productID': int(rec_pid), 
                    'productName': name
                })
            else:
                print(" Önerilen ProductID bulunamadı.")


        print(f" Önerilen ürünler (Id + İsim): {final_recommendations}")

        return jsonify(final_recommendations)

    except Exception as e:
        import traceback 
        print(f" Hata: {str(e)}")
        print(f"Traceback: {traceback.format_exc()}")
        return jsonify({'message': f'Sunucuda bir hata oluştu: {str(e)}'}), 500 