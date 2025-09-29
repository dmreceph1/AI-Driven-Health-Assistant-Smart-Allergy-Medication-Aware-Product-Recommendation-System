using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Şifre zorunludur.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Ad zorunludur.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email zorunludur.")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Telefon zorunludur.")]
		[Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
		public string Telefon { get; set; }

		[Required(ErrorMessage = "Cinsiyet zorunludur.")]
		public bool Cinsiyet { get; set; }
	}
}
