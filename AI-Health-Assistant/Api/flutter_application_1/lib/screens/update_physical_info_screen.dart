import 'package:flutter/material.dart';
import '../services/user_physical_info_service.dart';

class UpdatePhysicalInfoScreen extends StatefulWidget {
  final double? initialHeight;
  final double? initialWeight;

  const UpdatePhysicalInfoScreen({Key? key, this.initialHeight, this.initialWeight}) : super(key: key);

  @override
  _UpdatePhysicalInfoScreenState createState() => _UpdatePhysicalInfoScreenState();
}

class _UpdatePhysicalInfoScreenState extends State<UpdatePhysicalInfoScreen> {
  final _formKey = GlobalKey<FormState>();
  TextEditingController _heightController = TextEditingController();
  TextEditingController _weightController = TextEditingController();
  final UserPhysicalInfoService _physicalInfoService = UserPhysicalInfoService();
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _heightController.text = widget.initialHeight?.toString() ?? '';
    _weightController.text = widget.initialWeight?.toString() ?? '';
  }

  Future<void> _updatePhysicalInfo(BuildContext context) async {
    if (_formKey.currentState!.validate()) {
      setState(() {
        _isLoading = true;
      });

      final double? height = double.tryParse(_heightController.text);
      final double? weight = double.tryParse(_weightController.text);

      if (height != null && weight != null) {
        final success = await _physicalInfoService.updateUserPhysicalInfo(height: height, weight: weight);
        
        if (success) {
          final updatedInfo = await _physicalInfoService.getUserPhysicalInfo();
          if (updatedInfo['success']) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Fiziksel bilgileriniz başarıyla güncellendi.'),
                backgroundColor: Colors.green,
              ),
            );
            Navigator.of(context).pop(updatedInfo['data']);
          } else {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(updatedInfo['message'] ?? 'Bilgiler alınamadı.'),
                backgroundColor: Colors.red,
              ),
            );
          }
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Fiziksel bilgiler güncellenirken bir hata oluştu.'),
              backgroundColor: Colors.red,
            ),
          );
        }
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Geçerli boy ve kilo değerleri giriniz.'),
            backgroundColor: Colors.red,
          ),
        );
      }

      setState(() {
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Fiziksel Bilgileri Güncelle'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16.0),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    TextFormField(
                      controller: _heightController,
                      keyboardType: TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(labelText: 'Boy (m)'),
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Lütfen boyunuzu girin';
                        }
                        if (double.tryParse(value) == null) {
                          return 'Geçerli bir sayı girin';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 20),
                    TextFormField(
                      controller: _weightController,
                      keyboardType: TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(labelText: 'Kilo (kg)'),
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Lütfen kilonuzu girin';
                        }
                        if (double.tryParse(value) == null) {
                          return 'Geçerli bir sayı girin';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 30),
                    ElevatedButton(
                      onPressed: _isLoading ? null : () => _updatePhysicalInfo(context),
                      child: const Text('Kaydet'),
                    ),
                  ],
                ),
              ),
            ),
    );
  }
} 