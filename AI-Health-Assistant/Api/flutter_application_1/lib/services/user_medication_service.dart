import 'dart:convert';
import 'package:http/http.dart' as http;
import '../utils/constants.dart';
import 'auth_service.dart';

class UserMedicationService {
  final AuthService _authService = AuthService();

  Future<List<dynamic>> getUserMedications() async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return [];
    }

    final url = Uri.parse('$apiBaseUrl/UserMedications/$userId');
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
      print('Get user medications error: $e');
      return [];
    }
  }

  Future<List<dynamic>> getAllMedications() async {
    final String? token = await _authService.getToken();
    if (token == null) return [];

    final url = Uri.parse('$apiBaseUrl/Medication');
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
      print('Get all medications error: $e');
      return [];
    }
  }

  Future<bool> addUserMedication(int medicationId) async {
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();

    if (userId == null || token == null) {
      return false;
    }

    final url = Uri.parse('$apiBaseUrl/UserMedications');
    try {
      final response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'userID': userId,
          'medicationID': medicationId,
          'activeDate': DateTime.now().toIso8601String(),
          'inactiveDate': null,
        }),
      );

      return response.statusCode >= 200 && response.statusCode < 300;
    } catch (e) {
      print('Add user medication error: $e');
      return false;
    }
  }

  Future<bool> finishUserMedication(int userMedicationId) async {
    final String? token = await _authService.getToken();
    if (token == null) return false;

    final url = Uri.parse('$apiBaseUrl/UserMedications');
    try {
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode != 200) return false;

      final medications = jsonDecode(response.body);
      final medication = medications.firstWhere(
        (m) => m['userMedicationID'] == userMedicationId,
        orElse: () => null,
      );

      if (medication == null) return false;

      final updateResponse = await http.put(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'userMedicationID': medication['userMedicationID'],
          'userID': medication['userID'],
          'medicationID': medication['medicationID'],
          'activeDate': medication['activeDate'],
          'inactiveDate': DateTime.now().toIso8601String(),
        }),
      );

      return updateResponse.statusCode >= 200 && updateResponse.statusCode < 300;
    } catch (e) {
      print('Finish user medication error: $e');
      return false;
    }
  }
} 