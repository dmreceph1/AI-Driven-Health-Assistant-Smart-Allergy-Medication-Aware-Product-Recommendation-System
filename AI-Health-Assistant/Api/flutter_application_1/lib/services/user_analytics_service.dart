import 'dart:convert';
import 'package:flutter_application_1/services/auth_service.dart';
import 'package:http/http.dart' as http;

class UserAnalyticsService {
  final AuthService _authService = AuthService();

  Future<Map<String, dynamic>> getUserAnalytics() async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return {'success': false, 'message': 'Kullanıcı bilgileri bulunamadı.'};
    }

    final url = Uri.parse('http://localhost:5005/get_user_allergies_and_medications?user_id=$userId');
    try {
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        return {
          'success': true,
          'topAllergies': data['top_allergies'] ?? [],
          'topMedications': data['top_medications'] ?? [],
        };
      }
      return {'success': false, 'message': 'Veriler alınamadı.'};
    } catch (e) {
      print('Get user analytics error: $e');
      return {'success': false, 'message': 'Bir hata oluştu: $e'};
    }
  }
} 