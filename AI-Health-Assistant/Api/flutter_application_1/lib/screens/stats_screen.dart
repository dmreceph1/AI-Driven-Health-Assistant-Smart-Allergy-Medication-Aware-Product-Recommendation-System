import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';
import '../services/StatsService.dart';

class StatsScreen extends StatefulWidget {
  const StatsScreen({Key? key}) : super(key: key);

  @override
  State<StatsScreen> createState() => _StatsScreenState();
}

class _StatsScreenState extends State<StatsScreen> {
  final StatsService _statsService = StatsService();
  bool _isLoading = true;
  String _errorMessage = '';
  
  Map<String, dynamic>? _genderData;
  Map<String, dynamic>? _bmiData;
  Map<String, dynamic>? _allergiesData;
  Map<String, dynamic>? _medicationsData;
  List<dynamic>? _associationRules;

  @override
  void initState() {
    super.initState();
    _loadAllData();
  }

  Future<void> _loadAllData() async {
    setState(() {
      _isLoading = true;
      _errorMessage = '';
    });

    try {
      final results = await Future.wait([
        _statsService.getGenderDistribution(),
        _statsService.getBmiDistribution(),
        _statsService.getTopAllergies(),
        _statsService.getTopMedications(),
        _statsService.getAssociationRules(),
      ]);

      if (mounted) {
        setState(() {
          _genderData = results[0]['success'] ? results[0]['data'] : null;
          _bmiData = results[1]['success'] ? results[1]['data'] : null;
          _allergiesData = results[2]['success'] ? results[2]['data'] : null;
          _medicationsData = results[3]['success'] ? results[3]['data'] : null;
          _associationRules = results[4]['success'] ? results[4]['data'] : null;
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _errorMessage = 'Veri yüklenirken hata oluştu: $e';
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('İstatistikler'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadAllData,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _errorMessage.isNotEmpty
              ? Center(child: Text(_errorMessage, style: const TextStyle(color: Colors.red)))
              : RefreshIndicator(
                  onRefresh: _loadAllData,
                  child: SingleChildScrollView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.all(16.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (_genderData != null) _buildGenderChart(),
                        const SizedBox(height: 24),
                        if (_allergiesData != null) _buildAllergiesChart(),
                        const SizedBox(height: 24),
                        if (_medicationsData != null) _buildMedicationsChart(),
                        const SizedBox(height: 24),
                        if (_associationRules != null) _buildAssociationRulesTable(),
                      ],
                    ),
                  ),
                ),
    );
  }

  Widget _buildGenderChart() {
    final data = _genderData!;
    final colors = [Colors.blue, Colors.pink, Colors.grey];
    int index = 0;
    
    return _buildChartContainer(
      title: 'Cinsiyet Dağılımı',
      height: 300,
      chart: PieChart(
        PieChartData(
          sections: data.entries.map<PieChartSectionData>((entry) {
            final color = colors[index % colors.length];
            index++;
            return PieChartSectionData(
              color: color,
              value: entry.value.toDouble(),
              title: '${entry.key}\n${entry.value}',
              radius: 100,
              titleStyle: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            );
          }).toList(),
          sectionsSpace: 2,
          centerSpaceRadius: 40,
        ),
      ),
      legend: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          ...data.entries.map((entry) {
            final color = colors[data.entries.toList().indexOf(entry) % colors.length];
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 4.0),
              child: Row(
                children: [
                  Container(
                    width: 16,
                    height: 16,
                    color: color,
                  ),
                  const SizedBox(width: 8),
                  Text('${entry.key}: ${entry.value}'),
                ],
              ),
            );
          }),
        ],
      ),
    );
  }

  Widget _buildAllergiesChart() {
    final data = _allergiesData!;
    final allergies = data.keys.toList();
    
    return _buildChartContainer(
      title: 'En Yaygın Alerjiler',
      height: 300,
      chart: BarChart(
        BarChartData(
          alignment: BarChartAlignment.spaceAround,
          maxY: data.values.fold<double>(0, (max, value) => value > max ? value.toDouble() : max) * 1.2,
          barTouchData: BarTouchData(enabled: false),
          titlesData: FlTitlesData(
            show: true,
            bottomTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                getTitlesWidget: (value, meta) {
                  if (value < 0 || value >= allergies.length) return const Text('');
                  return Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: Text(
                      allergies[value.toInt()],
                      style: const TextStyle(fontSize: 10),
                    ),
                  );
                },
                reservedSize: 40,
              ),
            ),
            leftTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                reservedSize: 40,
                getTitlesWidget: (value, meta) {
                  if (value == 0) return const Text('');
                  return Text(
                    value.toInt().toString(),
                    style: const TextStyle(fontSize: 12),
                  );
                },
              ),
            ),
            topTitles: AxisTitles(sideTitles: SideTitles(showTitles: false)),
            rightTitles: AxisTitles(sideTitles: SideTitles(showTitles: false)),
          ),
          borderData: FlBorderData(show: false),
          barGroups: List.generate(
            allergies.length,
            (index) => BarChartGroupData(
              x: index,
              barRods: [
                BarChartRodData(
                  toY: data[allergies[index]].toDouble(),
                  color: Colors.redAccent,
                  width: 22,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(6),
                    topRight: Radius.circular(6),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildMedicationsChart() {
    final data = _medicationsData!;
    final medications = data.keys.toList();
    
    return _buildChartContainer(
      title: 'En Yaygın İlaçlar',
      height: 300,
      chart: BarChart(
        BarChartData(
          alignment: BarChartAlignment.spaceAround,
          maxY: data.values.fold<double>(0, (max, value) => value > max ? value.toDouble() : max) * 1.2,
          barTouchData: BarTouchData(enabled: false),
          titlesData: FlTitlesData(
            show: true,
            bottomTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                getTitlesWidget: (value, meta) {
                  if (value < 0 || value >= medications.length) return const Text('');
                  return Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: Text(
                      medications[value.toInt()],
                      style: const TextStyle(fontSize: 10),
                    ),
                  );
                },
                reservedSize: 40,
              ),
            ),
            leftTitles: AxisTitles(
              sideTitles: SideTitles(
                showTitles: true,
                reservedSize: 40,
                getTitlesWidget: (value, meta) {
                  if (value == 0) return const Text('');
                  return Text(
                    value.toInt().toString(),
                    style: const TextStyle(fontSize: 12),
                  );
                },
              ),
            ),
            topTitles: AxisTitles(sideTitles: SideTitles(showTitles: false)),
            rightTitles: AxisTitles(sideTitles: SideTitles(showTitles: false)),
          ),
          borderData: FlBorderData(show: false),
          barGroups: List.generate(
            medications.length,
            (index) => BarChartGroupData(
              x: index,
              barRods: [
                BarChartRodData(
                  toY: data[medications[index]].toDouble(),
                  color: Colors.blueAccent,
                  width: 22,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(6),
                    topRight: Radius.circular(6),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildAssociationRulesTable() {
  return _buildChartContainer(
    title: 'Birliktelik Kuralları',
    height: null,
    chart: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: 20),
        const Text(
          "Kuralların Açıklamaları:",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 10),
        ..._associationRules!.map((rule) {
          return Padding(
            padding: const EdgeInsets.only(bottom: 10.0),
            child: Text(
              "• ${rule['explanation']}",
              style: const TextStyle(fontSize: 14),
              softWrap: true,
              maxLines: null,
              overflow: TextOverflow.visible,
            ),
          );
        }).toList(),
      ],
    ),
  );
}
  String _formatList(List<dynamic> items) {
    return items.join(', ');
  }

  Widget _buildChartContainer({
    required String title,
    required Widget chart,
    double? height,
    Widget? legend,
  }) {
    return Card(
      elevation: 4,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            SizedBox(
              height: height,
              width: double.infinity,
              child: chart,
            ),
            if (legend != null) ...[
              const SizedBox(height: 16),
              legend,
            ],
          ],
        ),
      ),
    );
  }
}