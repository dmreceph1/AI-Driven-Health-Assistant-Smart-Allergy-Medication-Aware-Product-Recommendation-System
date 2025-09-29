from flask import Flask
from blueprints.oneri import oneri_bp
from blueprints.veri_madenciligi import veri_madenciligi_bp
from blueprints.health_score import health_score_bp
from blueprints.ocr import ocr_bp

app = Flask(__name__)

# Blueprintler
app.register_blueprint(oneri_bp, url_prefix='/oneri')
app.register_blueprint(veri_madenciligi_bp, url_prefix='/veri-madenciligi')
app.register_blueprint(health_score_bp, url_prefix='/health-score')
app.register_blueprint(ocr_bp, url_prefix='/ocr')

if __name__ == '__main__':
    print("Flask sunucusu başlatılıyor...")
    app.run(host='0.0.0.0', port=5000, debug=True, use_reloader=False) 