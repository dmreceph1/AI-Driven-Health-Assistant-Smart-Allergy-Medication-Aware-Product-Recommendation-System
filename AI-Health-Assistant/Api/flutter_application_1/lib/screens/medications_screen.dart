import 'package:flutter/material.dart';
import '../services/user_medication_service.dart';

class MedicationsScreen extends StatefulWidget {
  const MedicationsScreen({Key? key}) : super(key: key);

  @override
  _MedicationsScreenState createState() => _MedicationsScreenState();
}

class _MedicationsScreenState extends State<MedicationsScreen> {
  final UserMedicationService _medicationService = UserMedicationService();
  List<dynamic> _userMedications = [];
  List<dynamic> _allMedications = [];
  bool _isLoading = true;
  int? _selectedMedicationId;

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
      final userMedications = await _medicationService.getUserMedications();
      final allMedications = await _medicationService.getAllMedications();

      print('User Medications: $userMedications');
      print('All Medications: $allMedications');

      setState(() {
        _userMedications = userMedications;
        _allMedications = allMedications;
        _isLoading = false;
      });
    } catch (e) {
      print('Error loading data: $e');
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _addMedication() async {
    if (_selectedMedicationId == null) return;

    print('Adding medication with ID: $_selectedMedicationId');
    final success = await _medicationService.addUserMedication(_selectedMedicationId!);
    if (success) {
      await _loadData();
      setState(() {
        _selectedMedicationId = null;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('İlaç başarıyla eklendi.'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('İlaç eklenirken bir hata oluştu.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _finishMedication(int userMedicationId) async {
    final success = await _medicationService.finishUserMedication(userMedicationId);
    if (success) {
      await _loadData();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('İlaç başarıyla tamamlandı.'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('İlaç tamamlanırken bir hata oluştu.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  bool _isMedicationActive(dynamic medication) {
    final inactiveDate = medication['inactiveDate'];
    return inactiveDate == null || inactiveDate == "0001-01-01T00:00:00";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('İlaçlar'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'İlaç Ekle',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  DropdownButtonFormField<int>(
                    value: _selectedMedicationId,
                    decoration: const InputDecoration(
                      labelText: 'İlaç Seçiniz',
                      border: OutlineInputBorder(),
                    ),
                    items: [
                      const DropdownMenuItem(
                        value: null,
                        child: Text('Seçiniz...'),
                      ),
                      ..._allMedications.map((medication) {
                        print('Medication item: $medication');
                        return DropdownMenuItem(
                          value: medication['medicationID'] as int,
                          child: Text(medication['medicationName'] as String),
                        );
                      }).toList(),
                    ],
                    onChanged: (value) {
                      print('Selected value: $value');
                      setState(() {
                        _selectedMedicationId = value;
                      });
                    },
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: _selectedMedicationId == null ? null : _addMedication,
                    child: const Text('Ekle'),
                  ),
                  const SizedBox(height: 40),
                  const Text(
                    'Kayıtlı İlaç Bilgilerim',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  if (_userMedications.isEmpty)
                    const Center(
                      child: Text('Kayıtlı ilaç bulunmamaktadır.'),
                    )
                  else
                    ListView.builder(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: _userMedications.length,
                      itemBuilder: (context, index) {
                        final medication = _userMedications[index];
                        final isActive = _isMedicationActive(medication);
                        final activeDate = DateTime.parse(medication['activeDate'] as String);
                        final inactiveDate = medication['inactiveDate'] != null && 
                                           medication['inactiveDate'] != "0001-01-01T00:00:00"
                            ? DateTime.parse(medication['inactiveDate'] as String)
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
                                        medication['medicationName'] as String,
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
                                      'Başlangıç: ${activeDate.toString().split(' ')[0]}',
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 8),
                                Row(
                                  children: [
                                    const Icon(Icons.calendar_today, size: 16),
                                    const SizedBox(width: 8),
                                    Text(
                                      'Bitiş: ${inactiveDate != null ? inactiveDate.toString().split(' ')[0] : '-'}',
                                    ),
                                  ],
                                ),
                                if (isActive) ...[
                                  const SizedBox(height: 16),
                                  SizedBox(
                                    width: double.infinity,
                                    child: ElevatedButton(
                                      onPressed: () => _finishMedication(medication['userMedicationID'] as int),
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