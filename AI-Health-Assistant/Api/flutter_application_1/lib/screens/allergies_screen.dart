import 'package:flutter/material.dart';
import '../services/user_allergy_service.dart';

class AllergiesScreen extends StatefulWidget {
  const AllergiesScreen({Key? key}) : super(key: key);

  @override
  _AllergiesScreenState createState() => _AllergiesScreenState();
}

class _AllergiesScreenState extends State<AllergiesScreen> {
  final UserAllergyService _allergyService = UserAllergyService();
  List<dynamic> _userAllergies = [];
  List<dynamic> _allAllergies = [];
  bool _isLoading = true;
  int? _selectedAllergyId;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final userAllergies = await _allergyService.getUserAllergies();
      final allAllergies = await _allergyService.getAllAllergies();

      print('User Allergies: $userAllergies');
      print('All Allergies: $allAllergies');

      setState(() {
        _userAllergies = userAllergies;
        _allAllergies = allAllergies;
        _isLoading = false;
      });
    } catch (e) {
      print('Error loading data: $e');
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _addAllergy() async {
    if (_selectedAllergyId == null) return;

    print('Adding allergy with ID: $_selectedAllergyId');
    final success = await _allergyService.addUserAllergy(_selectedAllergyId!);
    if (success) {
      await _loadData();
      setState(() {
        _selectedAllergyId = null;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alerji başarıyla eklendi.'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alerji eklenirken bir hata oluştu.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _finishAllergy(int userAllergyId) async {
    final success = await _allergyService.finishUserAllergy(userAllergyId);
    if (success) {
      await _loadData();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alerji başarıyla tamamlandı.'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alerji tamamlanırken bir hata oluştu.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  bool _isAllergyActive(dynamic allergy) {
    final updateDate = allergy['updateDate'];
    return updateDate == null || updateDate == "0001-01-01T00:00:00";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Alerjiler'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Alerji Ekle',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  DropdownButtonFormField<int>(
                    value: _selectedAllergyId,
                    decoration: const InputDecoration(
                      labelText: 'Alerji Seçiniz',
                      border: OutlineInputBorder(),
                    ),
                    items: [
                      const DropdownMenuItem(
                        value: null,
                        child: Text('Seçiniz...'),
                      ),
                      ..._allAllergies.map((allergy) {
                        print('Allergy item: $allergy');
                        return DropdownMenuItem(
                          value: allergy['allergyID'] as int,
                          child: Text(allergy['allergyName'] as String),
                        );
                      }).toList(),
                    ],
                    onChanged: (value) {
                      print('Selected value: $value');
                      setState(() {
                        _selectedAllergyId = value;
                      });
                    },
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: _selectedAllergyId == null ? null : _addAllergy,
                    child: const Text('Ekle'),
                  ),
                  const SizedBox(height: 40),
                  const Text(
                    'Kayıtlı Alerji Bilgilerim',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  if (_userAllergies.isEmpty)
                    const Center(
                      child: Text('Kayıtlı alerji bulunmamaktadır.'),
                    )
                  else
                    ListView.builder(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: _userAllergies.length,
                      itemBuilder: (context, index) {
                        final allergy = _userAllergies[index];
                        final isActive = _isAllergyActive(allergy);
                        final diagnosisDate = DateTime.parse(allergy['diagnosisDate'] as String);
                        final updateDate = allergy['updateDate'] != null && 
                                         allergy['updateDate'] != "0001-01-01T00:00:00"
                            ? DateTime.parse(allergy['updateDate'] as String)
                            : null;

                        return Card(
                          margin: const EdgeInsets.only(bottom: 16),
                          color: isActive ? Colors.green.shade50 : Colors.red.shade50,
                          child: Padding(
                            padding: const EdgeInsets.all(16.0),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Row(
                                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                  children: [
                                    Expanded(
                                      child: Text(
                                        allergy['allergyName'] as String,
                                        style: const TextStyle(
                                          fontSize: 18,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                    ),
                                    Container(
                                      padding: const EdgeInsets.symmetric(
                                        horizontal: 12,
                                        vertical: 6,
                                      ),
                                      decoration: BoxDecoration(
                                        color: isActive ? Colors.green : Colors.red,
                                        borderRadius: BorderRadius.circular(20),
                                      ),
                                      child: Text(
                                        isActive ? 'Aktif' : 'Pasif',
                                        style: const TextStyle(
                                          color: Colors.white,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 12),
                                Row(
                                  children: [
                                    const Icon(Icons.calendar_today, size: 16),
                                    const SizedBox(width: 8),
                                    Text(
                                      'Başlangıç: ${diagnosisDate.toString().split(' ')[0]}',
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 8),
                                Row(
                                  children: [
                                    const Icon(Icons.calendar_today, size: 16),
                                    const SizedBox(width: 8),
                                    Text(
                                      'Bitiş: ${updateDate != null ? updateDate.toString().split(' ')[0] : '-'}',
                                    ),
                                  ],
                                ),
                                if (isActive) ...[
                                  const SizedBox(height: 16),
                                  SizedBox(
                                    width: double.infinity,
                                    child: ElevatedButton(
                                      onPressed: () => _finishAllergy(allergy['userAllergyID'] as int),
                                      style: ElevatedButton.styleFrom(
                                        backgroundColor: Colors.red,
                                        foregroundColor: Colors.white,
                                      ),
                                      child: const Text('Bitir'),
                                    ),
                                  ),
                                ],
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                ],
              ),
            ),
    );
  }
} 