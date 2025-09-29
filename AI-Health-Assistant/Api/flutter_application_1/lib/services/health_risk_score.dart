import 'dart:convert';
import 'package:http/http.dart' as http;
import 'auth_service.dart';

class HealthRiskService {
  final AuthService _authService = AuthService();

  Future<Map<String, dynamic>> getHealthRisk() async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return {'success': false, 'message': 'Kullanıcı ID veya token alınamadı.'};
    }

    final url = Uri.parse('http://10.0.2.2:5000/health-score/health-risk');
    final requestBody = jsonEncode({'UserID': userId});

    try {
      final response = await http.post(
        url,
        headers: {
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
        body: requestBody,
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {'success': true, 'data': responseData};
      } else {
        String errorMessage = 'Health risk verisi alınamadı. Kod: ${response.statusCode}';
        try {
          final errorData = jsonDecode(response.body);
          errorMessage = errorData['message'] ?? errorData['error'] ?? errorMessage;
        } catch (_) {}
        return {'success': false, 'message': errorMessage};
      }
    } catch (e) {
      return {'success': false, 'message': 'İstek hatası: $e'};
    }
  }
}
