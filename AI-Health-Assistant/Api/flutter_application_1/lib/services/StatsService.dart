import 'dart:convert';
import 'package:http/http.dart' as http;

class StatsService {
  final String baseUrl = 'http://10.0.2.2:5000/veri-madenciligi';

  Future<Map<String, dynamic>> getGenderDistribution() async {
    return _getRequest('$baseUrl/stats/gender');
  }

  Future<Map<String, dynamic>> getBmiDistribution() async {
    return _getRequest('$baseUrl/stats/bmi');
  }

  Future<Map<String, dynamic>> getTopAllergies() async {
    return _getRequest('$baseUrl/stats/top-allergies');
  }

  Future<Map<String, dynamic>> getTopMedications() async {
    return _getRequest('$baseUrl/stats/top-medications');
  }

  Future<Map<String, dynamic>> getAssociationRules() async {
    return _getRequest('$baseUrl/association-rules');
  }

  Future<Map<String, dynamic>> _getRequest(String url) async {
    try {
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        return {'success': true, 'data': responseData};
      } else {
        return {
          'success': false,
          'message': 'İstek başarısız oldu. Kod: ${response.statusCode}'
        };
      }
    } catch (e) {
      return {'success': false, 'message': 'Hata oluştu: $e'};
    }
  }
}
