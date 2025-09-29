import 'dart:convert';
import 'package:http/http.dart' as http;
import '../utils/constants.dart';
import 'auth_service.dart'; 

class UserPhysicalInfoService {
  final AuthService _authService = AuthService();

  Future<Map<String, dynamic>> getUserPhysicalInfo() async {
    print('=== getUserPhysicalInfo Başladı ===');
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();
    print('userId: $userId, token: ${token != null ? 'var' : 'yok'}');
    
    if (userId == null || token == null) {
      print('Kullanıcı ID veya token bulunamadı.');
      return {'success': false, 'message': 'Kullanıcı ID veya token bulunamadı.'};
    }

    final url = Uri.parse('$apiBaseUrl/UserPhysicalInfo/user/$userId');
    print('API URL: $url');

    try {
      print('GET isteği gönderiliyor...');
      final response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': 'Bearer $token',
        },
      );

      print('Response Status: ${response.statusCode}');
      print('Response Headers: ${response.headers}');
      print('Response Body: ${response.body}');

      if (response.statusCode == 200) {
        print('200 OK yanıtı alındı');
        final responseData = jsonDecode(response.body);
        print('Decoded Response Data: $responseData');
        return {'success': true, 'data': responseData};
      } else if (response.statusCode == 204) {
        print('204 No Content yanıtı alındı');
        return {'success': true, 'data': null};
      } else {
        print('Hata yanıtı alındı');
        String errorMessage = 'Fiziksel bilgiler alınamadı. Status Code: ${response.statusCode}';
        try {
          final responseData = jsonDecode(response.body);
          errorMessage = responseData['message'] ?? responseData['error'] ?? errorMessage;
          print('Hata mesajı: $errorMessage');
        } catch (e) {
          print("JSON Parse Error or no specific message: $e");
        }
        return {'success': false, 'message': errorMessage};
      }
    } catch (e, stackTrace) {
      print("Get User Physical Info Error: $e");
      print("Stack Trace: $stackTrace");
      return {'success': false, 'message': 'Bir hata oluştu: $e'};
    } finally {
      print('=== getUserPhysicalInfo Bitti ===');
    }
  }

  Future<bool> updateUserPhysicalInfo({required double height, required double weight}) async {
    print('=== updateUserPhysicalInfo Başladı ===');
    final int? userId = await _authService.getUserId();
    final String? token = await _authService.getToken();
    print('userId: $userId, token: ${token != null ? 'var' : 'yok'}');

    if (userId == null || token == null) {
      print('Kullanıcı ID veya token bulunamadı.');
      return false;
    }

    // Önce kullanıcının mevcut fiziksel bilgilerini al
    print('Mevcut fiziksel bilgiler alınıyor...');
    final currentInfo = await getUserPhysicalInfo();
    print('currentInfo: $currentInfo');

    if (!currentInfo['success']) {
      print('Mevcut fiziksel bilgiler alınamadı: ${currentInfo['message']}');
      return false;
    }

    // Kullanıcının mevcut infoID'sini bul
    int? infoID;
    if (currentInfo['data'] != null) {
      print('currentInfo data tipi: ${currentInfo['data'].runtimeType}');
      if (currentInfo['data'] is List) {
        print('List içeriği: ${currentInfo['data']}');
        for (var info in currentInfo['data']) {
          print('info: $info');
          if (info['userID'] == userId) {
            infoID = info['infoID'];
            print('infoID bulundu: $infoID');
            break;
          }
        }
      } else if (currentInfo['data'] is Map) {
        print('Map içeriği: ${currentInfo['data']}');
        if (currentInfo['data']['userID'] == userId) {
          infoID = currentInfo['data']['infoID'];
          print('infoID bulundu: $infoID');
        }
      }
    } else {
      print('currentInfo data null');
    }

    final url = Uri.parse('$apiBaseUrl/UserPhysicalInfo');
    print('API URL: $url');

    final requestBody = infoID != null
        ? {
            'infoID': infoID,
            'userID': userId,
            'height': height,
            'weight': weight
          }
        : {
            'userID': userId,
            'height': height,
            'weight': weight
          };

    print('Request Body: ${jsonEncode(requestBody)}');
    print('HTTP Metodu: ${infoID != null ? 'PUT' : 'POST'}');

    try {
      final response = infoID != null
          ? await http.put(  // Mevcut kayıt varsa PUT ile güncelle
              url,
              headers: <String, String>{
                'Content-Type': 'application/json; charset=UTF-8',
                'Authorization': 'Bearer $token',
              },
              body: jsonEncode(requestBody),
            )
          : await http.post(  // Yeni kayıt için POST
              url,
              headers: <String, String>{
                'Content-Type': 'application/json; charset=UTF-8',
                'Authorization': 'Bearer $token',
              },
              body: jsonEncode(requestBody),
            );

      print('Response Status: ${response.statusCode}');
      print('Response Headers: ${response.headers}');
      print('Response Body: ${response.body}');

      if (response.statusCode >= 200 && response.statusCode < 300) {
        print('İşlem başarılı');
        return true;
      } else {
        print('Fiziksel bilgiler güncellenemedi. Status Code: ${response.statusCode}, Body: ${response.body}');
        return false;
      }
    } catch (e, stackTrace) {
      print('Fiziksel bilgileri güncelleme hatası: $e');
      print('Stack Trace: $stackTrace');
      return false;
    } finally {
      print('=== updateUserPhysicalInfo Bitti ===');
    }
  }
}