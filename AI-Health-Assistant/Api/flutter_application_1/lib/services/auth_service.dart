import 'dart:convert'; 
import 'package:http/http.dart' as http; 
import 'package:shared_preferences/shared_preferences.dart'; 
import '../utils/constants.dart'; 

class AuthService {

  Future<Map<String, dynamic>> register({
    required String userName,
    required String password,
    required String name,
    required String email,
    required String telefon,
    required bool cinsiyet,
  }) async {
    final url = Uri.parse('$apiBaseUrl/Auth/register'); 
    try {
      final response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, dynamic>{
          'userName': userName, 
          'password': password,
          'name': name,
          'email': email,
          'telefon': telefon,
          'cinsiyet': cinsiyet,
        }),
      );

      if (response.statusCode >= 200 && response.statusCode < 300) {
        return {'success': true, 'message': 'Kayıt başarılı!'};
      } else {
        String errorMessage = 'Kayıt başarısız oldu. Status Code: ${response.statusCode}';
        try {
          final responseData = jsonDecode(response.body);
          errorMessage = responseData['message'] ?? responseData['error'] ?? errorMessage;
        } catch (e) {
          print("JSON Parse Error or no specific message: $e");
        }
        return {'success': false, 'message': errorMessage};
      }
    } catch (e) {
      print("Register Error: $e");
      return {'success': false, 'message': 'Bir hata oluştu: $e'};
    }
  }

  Future<Map<String, dynamic>> login({
    required String userName,
    required String password,
  }) async {
    final url = Uri.parse('$apiBaseUrl/Auth/login');
    try {
      final response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, String>{
          'userName': userName,
          'password': password,
        }),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        final String? token = responseData['token'];
        final int? userId = responseData['userId'];
        final String? userName = responseData['userName'];

        if (token != null && userId != null) {
          final prefs = await SharedPreferences.getInstance();
          await prefs.setString('authToken', token);
          await prefs.setInt('userId', userId);
          await prefs.setString('userName', userName ?? '');

          return {'success': true, 'token': token, 'userId': userId, 'userName': userName};
        } else {
           return {'success': false, 'message': 'API yanıtında token veya Kullanıcı Adı bulunamadı.'};
        }
      } else {
        
         String errorMessage = 'Giriş başarısız oldu. Status Code: ${response.statusCode}';
        try {
          final responseData = jsonDecode(response.body);
          errorMessage = responseData['message'] ?? responseData['error'] ?? errorMessage;
        } catch (e) {
          print("JSON Parse Error or no specific message: $e");
        }
        return {'success': false, 'message': errorMessage};
      }
    } catch (e) {
      
      print("Login Error: $e");
      return {'success': false, 'message': 'Bir hata oluştu: $e'};
    }
  }

  // Çıkış fonksiyonu
  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('authToken');
    await prefs.remove('userId');
    await prefs.remove('userName');
  }

  // Kayıtlı token kontrolü
  Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('authToken');
  }
  // Kayıtlı kullanıcı adını getirme
  Future<String?> getUsername() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final username = prefs.getString('userName');
      print('SharedPreferences username: $username'); // Debug için
      return username;
    } catch (e) {
      print('Get username error: $e');
      return null;
    }
  }

  // Kayıtlı kullanıcı ID'sini getirme
  Future<int?> getUserId() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt('userId');
  }
}