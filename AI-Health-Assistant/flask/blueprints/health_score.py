from flask import Blueprint, request, jsonify
import pyodbc

health_score_bp = Blueprint('health_score', __name__)

# SQL Server bağlantı cümlesi
conn_str = (
    r'DRIVER={SQL Server};'
    r'SERVER=DESKTOP-QAS58M6\SQLEXPRESS;'
    r'DATABASE=ApiProject;'
    r'Trusted_Connection=yes;'
)

@health_score_bp.route('/health-risk', methods=['POST'])
def health_risk():
    user_id = request.json['UserID']

    try:
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()

        # Boy ve kilo
        cursor.execute("SELECT Height, Weight FROM UserPhysicalInfo WHERE UserID = ?", user_id)
        result = cursor.fetchone()
        if result:
            height, weight = result
            bmi = weight / ((height) ** 2)
        else:
            return jsonify({"error": "Kullanıcı fiziksel bilgisi bulunamadı"}), 404

        # Alerji sayısı
        cursor.execute("SELECT COUNT(*) FROM UserAllergy WHERE UserID = ? AND UpdateDate IS NULL", user_id)
        allergy_count = cursor.fetchone()[0]

        # Aktif ilaç sayısı
        cursor.execute("""
            SELECT COUNT(*) FROM UserMedication
            WHERE UserID = ? AND InactiveDate IS NULL
        """, user_id)
        med_count = cursor.fetchone()[0]

        score = 100
        if bmi > 30:
            score -= 20
        elif bmi > 25:
            score -= 10

        score -= allergy_count * 5
        score -= med_count * 5

        yorum = (
            "Gayet iyi durumdasın!" if score >= 80 else
            "Genel olarak iyisin ama biraz daha dikkat etmelisin." if score >= 50 else
            "Sağlığına ciddi şekilde dikkat etmelisin."
            )

        return jsonify({
            "score": round(score, 2),
            "comment": yorum,
            "bmi": round(bmi, 1),
            "allergyCount": allergy_count,
            "medicationCount": med_count
        })

    except Exception as e:
        return jsonify({"error": str(e)}), 500 