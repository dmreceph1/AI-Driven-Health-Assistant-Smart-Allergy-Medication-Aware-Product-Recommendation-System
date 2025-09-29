// lib/screens/register_screen.dart
import 'package:flutter/material.dart';
import '../services/auth_service.dart'; 

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>(); 
  final _authService = AuthService(); 

  // TextEditingController'lar input alanlarındaki verileri tutar
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  final _telefonController = TextEditingController();
  bool _cinsiyet = true;
  bool _isLoading = false; // Yükleniyor durumunu takip etmek için

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    _nameController.dispose();
    _emailController.dispose();
    _telefonController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (_formKey.currentState!.validate()) {
      setState(() {
        _isLoading = true; 
      });

      final result = await _authService.register(
        userName: _usernameController.text,
        password: _passwordController.text,
        name: _nameController.text,
        email: _emailController.text,
        telefon: _telefonController.text,
        cinsiyet: _cinsiyet,
      );

       setState(() {
        _isLoading = false; 
      });

      if (!mounted) return; // Widget ağaçtan kaldırıldıysa işlem yapmaz

      if (result['success'] == true) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(result['message'] ?? 'Kayıt Başarılı! Giriş yapabilirsiniz.'), backgroundColor: Colors.green),
        );
        Navigator.of(context).pop();
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(result['message'] ?? 'Kayıt Başarısız!'), backgroundColor: Colors.red),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Kayıt Ol'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey, 
          child: SingleChildScrollView( // Ekran taşmasını önlemek için kaydırılabilir yapar
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                TextFormField(
                  controller: _usernameController,
                  decoration: const InputDecoration(labelText: 'Kullanıcı Adı'),
                  validator: (value) { 
                    if (value == null || value.isEmpty) {
                      return 'Kullanıcı adı boş olamaz';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _passwordController,
                  decoration: const InputDecoration(labelText: 'Şifre'),
                  obscureText: true, // Şifreyi gizler
                   validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Şifre boş olamaz';
                    }
                     if (value.length < 3) { 
                       return 'Şifre en az 3 karakter olmalı';
                     }
                    return null;
                  },
                ),
                 const SizedBox(height: 12),
                TextFormField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: 'Ad Soyad'),
                   validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Ad Soyad boş olamaz';
                    }
                    return null;
                  },
                ),
                 const SizedBox(height: 12),
                 TextFormField(
                  controller: _emailController,
                  decoration: const InputDecoration(labelText: 'E-posta'),
                  keyboardType: TextInputType.emailAddress,
                   validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'E-posta boş olamaz';
                    }
                    if (!value.contains('@')) { 
                      return 'Geçerli bir e-posta girin';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                 TextFormField(
                  controller: _telefonController,
                  decoration: const InputDecoration(labelText: 'Telefon'),
                  keyboardType: TextInputType.phone,
                   validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Telefon boş olamaz';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                SwitchListTile( 
                  title: const Text('Cinsiyet (Aktif = True)'), 
                  value: _cinsiyet,
                  onChanged: (bool value) {
                    setState(() {
                      _cinsiyet = value;
                    });
                  },
                ),
                const SizedBox(height: 20),
                _isLoading
                    ? const CircularProgressIndicator() 
                    : ElevatedButton(
                        onPressed: _register, 
                        child: const Text('Kayıt Ol'),
                      ),
                 const SizedBox(height: 20),
                 TextButton(
                  onPressed: () {
                    Navigator.of(context).pop(); 
                  },
                  child: const Text('Zaten hesabın var mı? Giriş Yap'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}