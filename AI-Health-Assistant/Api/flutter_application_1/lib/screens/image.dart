import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'dart:convert';
import '../utils/constants.dart';
import '../services/auth_service.dart';


class ProductSearchScreen extends StatefulWidget {
  const ProductSearchScreen({Key? key}) : super(key: key);
  @override
  _ProductSearchScreenState createState() => _ProductSearchScreenState();
}

class _ProductSearchScreenState extends State<ProductSearchScreen> {
  File? _selectedImage;
  final ImagePicker _picker = ImagePicker();
  String _searchMode = ""; // "image" veya "barcode" olarak modlar
  String? _productNameResult; // Sadece ürün adını tutacak
  List<String> _productContents = [];
  TextEditingController _productNameController = TextEditingController();
  final AuthService _authService = AuthService();
  bool _hasAllergyWarningShown = false;
  bool _hasMedicationWarningShown = false;
  int? selectedProductID;
  List<String> conflictingContents = [];


  // resim seçme menüsü
  Future<void> _showImageSourceDialog(Function(XFile) onImagePicked) async {
    showModalBottomSheet(
      context: context,
      builder: (_) => Wrap(
        children: [
          ListTile(
            leading: Icon(Icons.camera_alt),
            title: Text("Kameradan Çek"),
            onTap: () async {
              Navigator.pop(context);
              final pickedFile = await _picker.pickImage(source: ImageSource.camera);
              if (pickedFile != null) onImagePicked(pickedFile);
            },
          ),
          ListTile(
            leading: Icon(Icons.photo),
            title: Text("Galeriden Seç"),
            onTap: () async {
              Navigator.pop(context);
              final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
              if (pickedFile != null) onImagePicked(pickedFile);
            },
          ),
        ],
      ),
    );
  }

  // görsel seçildiğinde tetiklenen fonksiyon
  void _onImagePicked(XFile image) {
    setState(() {
      _selectedImage = File(image.path);
      _productNameResult = null;
      _productContents = [];
    });

    sendImageToApi(File(image.path), _searchMode);
  }

  // seçili resimi python API'ye gönderir
  Future<void> sendImageToApi(File imageFile, String mode) async {
    final uri = Uri.parse('http://10.0.2.2:5000/ocr/analyze');
    final request = http.MultipartRequest('POST', uri)
      ..fields['mode'] = mode
      ..files.add(await http.MultipartFile.fromPath('image', imageFile.path));

    final response = await request.send();

    if (response.statusCode == 200) {
      final respStr = await response.stream.bytesToString();
      final jsonResponse = jsonDecode(respStr);

      if (jsonResponse['mode'] == 'barcode') {
        final barcode = jsonResponse['text'];
        _fetchProductIdByBarcode(barcode);
      } else if (jsonResponse['mode'] == 'image') {
        final ocrText = jsonResponse['text'];
        print("OCR Çıktısı: $ocrText");
        _fetchProductIdByName(ocrText);
      }
    } else {
      setState(() {
        _productNameResult = "API Hatası: ${response.statusCode}";
      });
    }
  }
  // filtreleme API'si öneri 
  Future<void> _filterProducts(List<String> contents, int selectedProductID) async {
    print('\n==== Filtreleme Başlatılıyor ====');
    final filterUrl = Uri.parse('http://10.0.2.2:5000/oneri/filterProducts');
    
    // ek filtre için userid
    final int? userId = await _authService.getUserId();
    print('Kullanıcı ID: $userId');
    
    final response = await http.post(
      filterUrl,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'contents': contents,
        'productID': selectedProductID,
        'userId': userId,
      }),
    );

    if (response.statusCode == 200) {
      final List<dynamic> recommendedProducts = jsonDecode(response.body);
      print('Filtrelenmiş ürünler: $recommendedProducts');   
      if (recommendedProducts.isNotEmpty) {
        _showRecommendedProducts(recommendedProducts);
      } else{
        print('Hiçbir ürün önerisi bulunamadı.');
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Filtreleme sonucu önerilen ürün bulunamadı.")),
        );
      }
    } else {
      print('Filtreleme API yanıtı başarısız: ${response.statusCode}');
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Filtreleme işlemi başarısız oldu.")),
      );
    }
    print('==== Filtreleme Tamamlandı ====');
  }
  //önerilen ürünleri gösterir
  void _showRecommendedProducts(List<dynamic> recommendedProducts) {
    String recommendationMessage = "Önerilen Ürünler:\n"; 
    for (var product in recommendedProducts) {
      recommendationMessage += "- ${product['productName']}\n";
    }
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text("Ürün Önerileri"),
        content: Text(recommendationMessage),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("Tamam"),
          ),
        ],
      ),
    );
  }

  // .NET API'den barkod ile ProductID getiren endpoint
  Future<void> _fetchProductIdByBarcode(String barcode) async {
    final url = Uri.parse('$apiBaseUrl/Products/getByBarcode?barcode=$barcode');
    try {
      final response = await http.get(url);
      if (response.statusCode == 200) {
        final product = jsonDecode(response.body);
        setState(() {
          _productNameResult = product['productName'];
        });
        print("_fetchProductIdByBarcode - Product ID Alındı: ${product['productId']}");
        fetchProductContents(product['productID']);
        selectedProductID = product['productID'];
      } else {
        setState(() {
          _productNameResult = "Ürün bulunamadı";
          _productContents = [];
        });
      }
    } catch (e) {
      setState(() {
        _productNameResult = "Hata oluştu: $e";
        _productContents = [];
      });
    }
  }

  // .NET API'den ürün adı ile ProductID getiren endpoint
  Future<void> _fetchProductIdByName(String productName) async {
    final url = Uri.parse('$apiBaseUrl/Products/getProductIdByName?productName=$productName');
    print("_fetchProductIdByName - İstek URL'i (getProductIdByName): $url");
    try {
      final response = await http.get(url);
      print("_fetchProductIdByName - Status Code (getProductIdByName): ${response.statusCode}"); 
      print("_fetchProductIdByName - Response Body (getProductIdByName): ${response.body}");   
      if (response.statusCode == 200) {
        final productIdResponse = jsonDecode(response.body);
        final int? productId = productIdResponse['productId'];
        print("_fetchProductIdByName - Alınan Product ID: $productId");
        final productInfoUrl = Uri.parse('$apiBaseUrl/Products/getByName?productName=$productName');
        final productInfoResponse = await http.get(productInfoUrl);
        if (productInfoResponse.statusCode == 200) {
          final productInfo = jsonDecode(productInfoResponse.body);
          setState(() {
            _productNameResult = productInfo['productName'];
          });
        }
        fetchProductContents(productId);
        selectedProductID = productId;
      } else {
        setState(() {
          _productNameResult = "Ürün bulunamadı.";
          _productContents = [];
        });
      }
    } catch (e) {
      setState(() {
        _productNameResult = "Hata oluştu: $e";
        _productContents = [];
      });
    }
  }

  // .NET API'den ProductID ile ürün içeriklerini getirir
 Future<void> fetchProductContents(int? productId) async {
   print('\n=== fetchProductContents Başladı ===');
   print('Product ID: $productId');
   
   if (productId == null) {
     print('HATA: Geçersiz ürün ID');
     setState(() {
       _productContents = ["Geçersiz ürün ID"];
     });
     return;
   }
   
   final url = Uri.parse('$apiBaseUrl/ProductContents/GetContentsByProductId?productId=$productId');
   print('ProductContents API URL: $url');
   
   try {
     print('ProductContents API isteği gönderiliyor...');
     final response = await http.get(url);
     print('ProductContents API Response Status: ${response.statusCode}');
     print('ProductContents API Response Body: ${response.body}');
     
     if (response.statusCode == 200) {
       final String contentsString = jsonDecode(response.body)[0];
       
       setState(() {
         _productContents = contentsString.split(',');
         _productContents = _productContents.map((e) => e.trim()).toList();
       });
       
       _showProductDetailsBottomSheet();
     } else {
       setState(() {
         _productContents = ["İçerikler bulunamadı"];
       });
     }
   } catch (e) {
     setState(() {
       _productContents = ["İçerikler yüklenirken hata oluştu: $e"];
     });
   }
   print('=== fetchProductContents Bitti ===\n');
 }
//alerji kontrol
Future<void> _checkAllergies(List<String> contents) async {
  print('\n=== _checkAllergies Başladı ===');
  print('Kontrol edilecek içerikler: $contents');
  List<Map<String, dynamic>> matchingAllergies = [];
  
  for (String content in contents) {
    print('\nİçerik kontrol ediliyor: $content');
    final allergyUrl = Uri.parse('$apiBaseUrl/Allergy/getAllergyByMedicationContent?content=$content');
    print('Allergy API URL: $allergyUrl');
    
    try {
      print('Allergy API isteği gönderiliyor...');
      final response = await http.get(allergyUrl);
      print('Allergy API Response Status: ${response.statusCode}');
      print('Allergy API Response Body: ${response.body}');
      
      if (response.statusCode == 200) {
        final List<dynamic> allergies = jsonDecode(response.body);
        print('Bulunan alerjiler: $allergies');
        
        for (var allergy in allergies) {
          print('Alerji eklendi: ${allergy['allergyName']} (ID: ${allergy['allergyID']})');
          matchingAllergies.add({
            'allergyID': allergy['allergyID'],
            'allergyName': allergy['allergyName'],
            'content': content
          });
        }
      } else {
        print('Bu içerik için alerji bulunamadı');
      }
    } catch (e) {
      print("Allergy check error: $e");
    }
  }
  
  print('\nToplam eşleşen alerjiler: ${matchingAllergies.length}');
  print('Eşleşen alerjiler detay: $matchingAllergies');
  
  if (matchingAllergies.isNotEmpty) {
    final int? userId = await _authService.getUserId();
    print('\nKullanıcı ID: $userId');
    
    if (userId == null) {
      print('HATA: Kullanıcı ID bulunamadı');
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Kullanıcı girişi yapılmamış")),
      );
      return;
    }
    
    List<Map<String, dynamic>> activeAllergies = [];
    
    for (var allergy in matchingAllergies) {     
      final userAllergyUrl = Uri.parse('$apiBaseUrl/UserAllergy/getAllergiesByUserIdAndAllergyId?userId=$userId&allergyId=${allergy['allergyID']}');
      try {
        final response = await http.get(userAllergyUrl);
        print('>>> User Allergy Check Status: ${response.statusCode}');
        print('>>> User Allergy Check Body: ${response.body}');
        if (response.statusCode == 200) {
          final bool hasAllergy = jsonDecode(response.body);
          print('Kullanıcının bu alerjisi var mı: $hasAllergy');
          
          if (hasAllergy) {
            final allergyDetailsUrl = Uri.parse('$apiBaseUrl/UserAllergy/$userId/allergy/${allergy['allergyID']}');  
            print('Allergy Details API URL: $allergyDetailsUrl');  
            print('Kullanıcı ID: $userId, Alerji ID: ${allergy['allergyID']}');  

            final detailsResponse = await http.get(allergyDetailsUrl);  
  
            print('Alerji Detayları API Yanıt Durum Kodu: ${detailsResponse.statusCode}');  
            print('Alerji Detayları API Yanıtı: ${detailsResponse.body}');  

            if (detailsResponse.statusCode == 200) {
              final details = jsonDecode(detailsResponse.body);
              final String updateDate = details['updateDate']?.toString() ?? "";
              
              if (updateDate == null || updateDate == "0001-01-01T00:00:00" || updateDate == "") {
               print('AKTİF ALERJİ BULUNDU!');
               print('Alerji adı: ${allergy['allergyName']}');
               print('İçerik: ${allergy['content']}');
               activeAllergies.add({
                'allergyName': allergy['allergyName'],
                'content': allergy['content']
              });
              } else {
                print('Alerji aktif değil, güncelleme tarihi: $updateDate');
            }
          } else {
              print('HATA: AllergyDetails API yanıt kodu: ${detailsResponse.statusCode}');
              print('Hata mesajı: ${detailsResponse.body}');
            }
          }
        }
      } catch (e) {
        print("User allergy check error: $e");
      }
    }
        
    if (activeAllergies.isNotEmpty) {
      print('Alerji uyarısı gösteriliyor');
      _showAllergyWarning(activeAllergies);
    } else {
      print('Aktif alerji bulunamadı');
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Bu ürün için aktif alerjiniz bulunmamaktadır")),
      );
    }
  } else {
    print('Eşleşen alerji bulunamadı');
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Bu ürün için alerjiniz bulunmamaktadır")),
    );
  }
  print('=== _checkAllergies Bitti ===\n');
}
//ilac kontrol
Future<List<Map<String, dynamic>>> _checkMedications(List<String> contents) async {
  print('\n=== _checkMedications Başladı ===');
  print('Kontrol edilecek içerikler: $contents');
  List<Map<String, dynamic>> matchingMedications = [];

  for (String content in contents) {
    print('\nİlaç İçeriği kontrol ediliyor: $content');
    final medicationUrl = Uri.parse('$apiBaseUrl/Medication/getMedicationByIlacContent?content=$content');
    print('Medication API URL: $medicationUrl');

    try {
      print('Medication API isteği gönderiliyor...');
      final response = await http.get(medicationUrl);
      print('Medication API Response Status: ${response.statusCode}');
      print('Medication API Response Body: ${response.body}');

      if (response.statusCode == 200) {
        final List<dynamic> medications = jsonDecode(response.body);
        print('Bulunan ilaç eşleşmeleri: $medications');

        for (var med in medications) {
          matchingMedications.add({
            'medicationID': med['medicationID'],
            'medicationName': med['medicationName'],
            'content': content
          });
        }
      } else {
        print('Bu içerik için ilaç eşleşmesi bulunamadı');
      }
    } catch (e) {
      print("Medication check error: $e");
    }
  }

  final int? userId = await _authService.getUserId();
  if (userId == null) {
    print('HATA: Kullanıcı ID bulunamadı');
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Kullanıcı girişi yapılmamış")),
    );
    return [];
  }

  List<Map<String, dynamic>> activeMedications = [];

  for (var med in matchingMedications) {
    final checkUrl = Uri.parse('$apiBaseUrl/UserMedications/getMedicationsByUserIdAndAllergyId?userId=$userId&medicationId=${med['medicationID']}');
    print('Kullanıcı ilaç kontrol URL: $checkUrl');

    try {
      final response = await http.get(checkUrl);
      print('Kullanıcı ilaç kontrol yanıtı: ${response.statusCode} - ${response.body}');
      if (response.statusCode == 200) {
        final bool hasMedication = jsonDecode(response.body);
        if (hasMedication) {
          final detailUrl = Uri.parse('$apiBaseUrl/UserMedications/$userId/medication/${med['medicationID']}');
          print('İlaç detay kontrol URL: $detailUrl');

          final detailResponse = await http.get(detailUrl);
          print('Detay yanıtı: ${detailResponse.statusCode} - ${detailResponse.body}');
          if (detailResponse.statusCode == 200) {
            final details = jsonDecode(detailResponse.body);
            final String inactivateDate = details['inactivateDate']?.toString() ?? "";

            if (inactivateDate == null || inactivateDate == "" || inactivateDate == "0001-01-01T00:00:00") {
              print('AKTİF İLAÇ BULUNDU: ${med['medicationName']}');
              activeMedications.add({
                'medicationName': med['medicationName'],
                'content': med['content']
              });
            } else {
              print('İlaç aktif değil, Bitiş Tarihi: $inactivateDate');
            }
          }
        }
      }
    } catch (e) {
      print("İlaç kontrol hatası: $e");
    }
  }

  print('=== Aktif ilaçlar: $activeMedications ===');
  return activeMedications;
}


void _showAllergyWarning(List<Map<String, dynamic>> activeAllergies) {
  String warningMessage = "Bu ürünü kullanmamanız önerilir çünkü şu alerjilerinizle çakışıyor:\n";
  for (var allergy in activeAllergies) {
    conflictingContents.add(allergy['content']);
    warningMessage += "- ${allergy['allergyName']} (İçerik: ${allergy['content']})\n";
  }
  
  showDialog(
    context: context,
    builder: (context) => AlertDialog(
      title: Text("ALERJİ UYARISI"),
      content: Text(warningMessage),
      actions: [
        TextButton(
          onPressed: () {
            setState(() {
              _hasAllergyWarningShown = true;  
            });
            Navigator.pop(context);
          },
          child: Text("Tamam"),
        ),
      ],
    ),
  );
}
  // Ürün adı ile textbox'da arama
  void _searchByProductName() {
    setState(() {
      _selectedImage = null; 
      _productNameResult = null; 
      _productContents = [];    
    });
    final productName = _productNameController.text.trim();
    if (productName.isNotEmpty) {
      _fetchProductIdByName(productName);
    } else {
      setState(() {
        _productNameResult = "Lütfen bir ürün adı girin.";
        _productContents = [];
      });
    }
  }

  void _showMedicationWarning(List<Map<String, dynamic>> activeMedications) {
  String warningMessage = "Bu ürünü kullanmamanız önerilir çünkü şu ilaçlarla çakışıyor:\n";
  for (var med in activeMedications) {
    conflictingContents.add(med['content']);
    warningMessage += "- ${med['medicationName']} (İçerik: ${med['content']})\n";
  }

  showDialog(
    context: context,
    builder: (context) => AlertDialog(
      title: Text("İLAÇ UYARISI"),
      content: Text(warningMessage),
      actions: [
        TextButton(
          onPressed: () {
            setState(() {
              _hasMedicationWarningShown = true; 
            });
            Navigator.pop(context);
          },
          child: Text("Tamam"),
        ),
      ],
    ),
  );
}
Future<void> _checkAllergyAndMedication(List<String> contents) async {
  print('\n==== Tüm Kontroller Başlatılıyor ====');
  await _checkAllergies(contents);
  List<Map<String, dynamic>> meds = await _checkMedications(contents);
  if (meds.isNotEmpty) {
    _showMedicationWarning(meds);
    _hasMedicationWarningShown = true;
  } else {
    print('İlaç uyarısı yok, aktif ilaç bulunamadı.');
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("Bu ürün ile eşleşen aktif ilaç tedaviniz bulunmamaktadır")),
    );
  }
  if (_hasAllergyWarningShown || _hasMedicationWarningShown) {
    _showSuggestionsPanelBottom();
  }
  print('\n==== Tüm Kontroller Tamamlandı ====');
}
  void _showSuggestionsPanelBottom() async {
    if(conflictingContents.isEmpty){
      return;
    }
      await showModalBottomSheet(
        context: context,
        builder: (context) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text("Önerileri Görmek İçin Tıklayın"),
                SizedBox(height: 10),
                ElevatedButton(
                  onPressed: () {
                    // Öneri fonksiyonu
                    _filterProducts(conflictingContents, selectedProductID!);  
                    Navigator.pop(context); 
                  },
                  child: Text("Önerileri Göster"),
                ),
              ],
            ),
          );
        },
      );
      conflictingContents.clear();
    }

  void _showProductDetailsBottomSheet() {
  showModalBottomSheet(
    context: context,
    isScrollControlled: true,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
    ),
    builder: (BuildContext context) {
      return DraggableScrollableSheet(
        expand: false,
        builder: (_, controller) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      "Ürün Bilgisi",
                      style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                    ),
                    IconButton(
                      icon: Icon(Icons.close),
                      onPressed: () => Navigator.pop(context),
                    )
                  ],
                ),
                SizedBox(height: 10),
                if (_productNameResult != null)
                  Text(
                    _productNameResult!,
                    style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: Colors.indigo),
                    textAlign: TextAlign.center,
                  ),
                SizedBox(height: 10),
                if (_productContents.isNotEmpty)
                  Expanded(
                    child: ListView.builder(
                      controller: controller,
                      itemCount: _productContents.length,
                      itemBuilder: (context, index) {
                        return ListTile(
                          title: Text("- ${_productContents[index]}"),
                        );
                      },
                    ),
                  ),
                if (_productContents.isEmpty)
                  Text("İçerikler bulunamadı", style: TextStyle(fontStyle: FontStyle.italic)),
                SizedBox(height: 16),
                ElevatedButton.icon(
                  onPressed: () async {
                    Navigator.pop(context); 
                    await _checkAllergyAndMedication(_productContents);
                  },
                  icon: Icon(Icons.check_circle),
                  label: Text("Uygun mu?"),
                ),
              ],
            ),
          );
        },
      );
    },
  );
}

@override
Widget build(BuildContext context) {
  return Scaffold(
    appBar: AppBar(title: Text("Ürün Ara")),
    body: Padding(
      padding: const EdgeInsets.all(20.0),
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(height: 20),
            ElevatedButton.icon(
              icon: Icon(Icons.qr_code_scanner),
              label: Text("Barkod ile Ara"),
              onPressed: () {
                _searchMode = "barcode";
                _showImageSourceDialog(_onImagePicked);
              },
            ),
            SizedBox(height: 20),
            TextField(
              controller: _productNameController,
              decoration: InputDecoration(
                labelText: "Ürün Adı ile Ara",
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 10),
            ElevatedButton(
              onPressed: _searchByProductName,
              child: Text("Ara"),
            ),
            SizedBox(height: 30),
            if (_selectedImage != null)
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text("Seçilen Görsel:", style: TextStyle(fontWeight: FontWeight.bold)),
                  SizedBox(height: 10),
                  Image.file(
                    _selectedImage!,
                    height: 200,
                    width: double.infinity, 
                    fit: BoxFit.contain, 
                  ),
                  SizedBox(height: 20),
                ],
              ),
            if (_productNameResult == "Ürün bulunamadı")
              Padding(
                padding: const EdgeInsets.only(top: 20.0),
                child: Text(
                  "Ürün bulunamadı",
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.red),
                ),
              ),
            if (_productNameResult?.startsWith("Ürün Bulunamadı") == true)
              Padding(
                padding: const EdgeInsets.only(top: 20.0),
                child: Text(
                  _productNameResult!,
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.red),
                ),
              ),
            if (_productNameResult == "Lütfen bir ürün adı girin")
              Padding(
                padding: const EdgeInsets.only(top: 20.0),
                child: Text(
                  _productNameResult!,
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.orange),
                ),
              ),
          ],
        ),
      ),
    ),
  );
}
}