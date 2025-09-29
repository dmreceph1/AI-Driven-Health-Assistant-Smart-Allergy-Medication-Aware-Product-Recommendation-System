import 'dart:convert';
import 'package:http/http.dart' as http;
import '../utils/constants.dart';
import 'auth_service.dart';

class UserAllergyService {
  final AuthService _authService = AuthService();

  Future<List<dynamic>> getUserAllergies() async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return [];
    }

    final url = Uri.parse('$apiBaseUrl/UserAllergy/$userId');
    try {
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      }
      return [];
    } catch (e) {
      print('Get user allergies error: $e');
      return [];
    }
  }

  Future<List<dynamic>> getAllAllergies() async {
    final String? token = await _authService.getToken();
    if (token == null) return [];

    final url = Uri.parse('$apiBaseUrl/Allergy');
    try {
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      }
      return [];
    } catch (e) {
      print('Get all allergies error: $e');
      return [];
    }
  }

  Future<bool> addUserAllergy(int allergyId) async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return false;
    }

    final url = Uri.parse('$apiBaseUrl/UserAllergy');
    try {
      final response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'userID': userId,
          'allergyID': allergyId,
          'diagnosisDate': DateTime.now().toIso8601String(),
          'updateDate': null,
        }),
      );

      return response.statusCode >= 200 && response.statusCode < 300;
    } catch (e) {
      print('Add user allergy error: $e');
      return false;
    }
  }

  Future<bool> finishUserAllergy(int userAllergyId) async {
    final String? token = await _authService.getToken();
    if (token == null) return false;

    final url = Uri.parse('$apiBaseUrl/UserAllergy');
    try {
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode != 200) return false;

      final allergies = jsonDecode(response.body);
      final allergy = allergies.firstWhere(
        (a) => a['userAllergyID'] == userAllergyId,
        orElse: () => null,
      );

      if (allergy == null) return false;

      final updateResponse = await http.put(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'userAllergyID': allergy['userAllergyID'],
          'userID': allergy['userID'],
          'allergyID': allergy['allergyID'],
          'diagnosisDate': allergy['diagnosisDate'],
          'updateDate': DateTime.now().toIso8601String(),
        }),
      );

      return updateResponse.statusCode >= 200 && updateResponse.statusCode < 300;
    } catch (e) {
      print('Finish user allergy error: $e');
      return false;
    }
  }
} 