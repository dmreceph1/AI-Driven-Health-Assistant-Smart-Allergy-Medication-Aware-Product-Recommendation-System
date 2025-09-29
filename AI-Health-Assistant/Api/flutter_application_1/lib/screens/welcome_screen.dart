import 'package:flutter/material.dart';
import 'package:flutter_application_1/screens/image.dart';
import '../services/auth_service.dart';
import '../services/user_physical_info_service.dart';
import 'login_screen.dart';
import 'update_physical_info_screen.dart';
import 'allergies_screen.dart';
import 'medications_screen.dart';
import 'package:fl_chart/fl_chart.dart';  
import '../services/health_risk_score.dart';
import 'stats_screen.dart';

class WelcomeScreen extends StatefulWidget {
  const WelcomeScreen({Key? key}) : super(key: key);

  @override
  _WelcomeScreenState createState() => _WelcomeScreenState();
}

class _WelcomeScreenState extends State<WelcomeScreen> {
  final AuthService _authService = AuthService();
  final UserPhysicalInfoService _physicalInfoService = UserPhysicalInfoService();
  final HealthRiskService _healthRiskService = HealthRiskService();

  bool _isLoading = true;
  Map<String, dynamic>? _physicalInfo;
  Map<String, dynamic>? _healthRiskData;
  bool _isLoadingHealthRisk = true;

  @override
  void initState() {
    super.initState();
    _loadUserData();
    _loadHealthRisk();
  }

  Future<void> _loadHealthRisk() async {
    if (!mounted) return;
    setState(() {
      _isLoadingHealthRisk = true;
    });

    final result = await _healthRiskService.getHealthRisk();

    if (mounted) {
      setState(() {
        if (result['success']) {
          _healthRiskData = result['data'];
        } else {
          _healthRiskData = null;
        }
        _isLoadingHealthRisk = false;
      });
    }
  }

  Future<void> _loadUserData() async {
    if (mounted) {
      setState(() {
        _isLoading = true;
      });
    }

    final physicalInfo = await _physicalInfoService.getUserPhysicalInfo();
    print('WelcomeScreen - Loaded physical info: $physicalInfo');

    if (mounted) {
      setState(() {
        _physicalInfo = physicalInfo['success'] ? physicalInfo['data'] : null;
        _isLoading = false;
      });
    }
  }

  Future<void> _logout() async {
    await _authService.logout();
    if (mounted) {
      Navigator.of(context).pushAndRemoveUntil(
        MaterialPageRoute(builder: (context) => const LoginScreen()),
        (Route<dynamic> route) => false,
      );
    }
  }

  void _navigateToUpdatePhysicalInfo() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => UpdatePhysicalInfoScreen(
          initialHeight: _physicalInfo?['height'],
          initialWeight: _physicalInfo?['weight'],
        ),
      ),
    ).then((result) {
      if (result != null && mounted) {
        setState(() {
          _physicalInfo = result;
        });
      }
    });
  }

  Widget _buildHealthRiskChart() {
  if (_isLoadingHealthRisk) {
    return const Center(child: CircularProgressIndicator());
  }

  if (_healthRiskData == null) {
    return const Text('Sağlık riski verisi alınamadı.');
  }

  final bmi = double.tryParse(_healthRiskData!['bmi'].toString()) ?? 0.0;
  final allergyCount = int.tryParse(_healthRiskData!['allergyCount'].toString()) ?? 0;
  final medicationCount = int.tryParse(_healthRiskData!['medicationCount'].toString()) ?? 0;
  final score = double.tryParse(_healthRiskData!['score'].toString()) ?? 0.0;

  Color scoreColor;
  if (score >= 80) {
    scoreColor = Colors.green; // İyi durumda
  } else if (score >= 50) {
    scoreColor = Colors.orange; // Dikkatli olunmalı
  } else {
    scoreColor = Colors.red; // Sağlık riski yüksek
  }

  return Card(
    elevation: 3,
    margin: const EdgeInsets.symmetric(vertical: 16),
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Sağlık Riski Grafiği',
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),

          Container(
            height: 20,
            child: LinearProgressIndicator(
              value: score / 100,
              backgroundColor: Colors.grey.shade300,
              valueColor: AlwaysStoppedAnimation(scoreColor),
            ),
          ),
          const SizedBox(height: 16),

          Text(
            'Skor: ${score.toStringAsFixed(2)}%',
            style: TextStyle(fontSize: 16, color: scoreColor),
          ),
          const SizedBox(height: 16),
          Text('Alerji Sayısı: $allergyCount'),
          Text('İlaç Sayısı: $medicationCount'),  
          const SizedBox(height: 10),
          Text(
            'Yorum: ${_healthRiskData!['comment'] ?? ''}',
            style: const TextStyle(fontSize: 16),
          ),
        ],
      ),
    ),
  );
}
 

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ana Sayfa'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
             _loadUserData();
             _loadHealthRisk();
            },
          ),
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: _logout,
          ),
        ],
      ),
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: [
            const DrawerHeader(
              decoration: BoxDecoration(
                color: Colors.blue,
              ),
              child: Text(
                'Menü',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Ana Sayfa'),
              onTap: () => Navigator.pop(context),
            ),
            ListTile(
              leading: const Icon(Icons.warning),
              title: const Text('Alerjiler'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const AllergiesScreen(),
                  ),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.medication),
              title: const Text('İlaçlar'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const MedicationsScreen(),
                  ),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.camera),
              title: const Text('Ürün Ara'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const ProductSearchScreen(),
                  ),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.bar_chart),
              title: const Text('İstatistikler'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const StatsScreen(),
                  ),
                );
              },
            ),
          ],
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16.0),
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Fiziksel Bilgiler',
                      style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(height: 10),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Card(
                            elevation: 3,
                            child: Padding(
                              padding: const EdgeInsets.all(16.0),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  const Text('Boy', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                                  const SizedBox(height: 8),
                                  Text(
                                    _physicalInfo != null && _physicalInfo!['height'] != null
                                        ? '${_physicalInfo!['height']} m'
                                        : 'Bilgi Yok',
                                    style: const TextStyle(fontSize: 16),
                                  ),
                                  const SizedBox(height: 12),
                                  ElevatedButton(
                                    onPressed: _navigateToUpdatePhysicalInfo,
                                    child: const Text('Güncelle'),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: Card(
                            elevation: 3,
                            child: Padding(
                              padding: const EdgeInsets.all(16.0),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  const Text('Kilo', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                                  const SizedBox(height: 8),
                                  Text(
                                    _physicalInfo != null && _physicalInfo!['weight'] != null
                                        ? '${_physicalInfo!['weight']} kg'
                                        : 'Bilgi Yok',
                                    style: const TextStyle(fontSize: 16),
                                  ),
                                  const SizedBox(height: 12),
                                  ElevatedButton(
                                    onPressed: _navigateToUpdatePhysicalInfo,
                                    child: const Text('Güncelle'),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                    _buildHealthRiskChart(),
                  ],
                ),
              ),
            ),
    );
  }
}
