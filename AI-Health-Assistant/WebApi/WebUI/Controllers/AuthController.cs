using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Controllers
{
	public class AuthController : Controller
	{
		private readonly AuthService _authService;
		private readonly IUserPhysicalInfoService _userPhysicalInfoService;
		private readonly IUserMedicationService _userMedicationService;

		public AuthController(AuthService authService, IUserPhysicalInfoService userPhysicalInfoService, IUserMedicationService userMedicationService)
		{
			_authService = authService;
			_userPhysicalInfoService = userPhysicalInfoService;
			_userMedicationService = userMedicationService;
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var authResponse = await _authService.LoginAsync(model);
			if (authResponse == null)
			{
				ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
				return View(model);
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, model.UserName),
				new Claim("Token", authResponse.Token),
				new Claim("UserId", authResponse.UserId.ToString())
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties { IsPersistent = true };

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

			// fizilsek bilgiler
			var physicalInfo = await _userPhysicalInfoService.GetUserPhysicalInfoAsync(authResponse.UserId);
			if (physicalInfo != null)
			{
				TempData["UserHeight"] = physicalInfo.Height.ToString();
				TempData["UserWeight"] = physicalInfo.Weight.ToString();
			}

			return RedirectToAction("Index", "Category");
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var (success, errorMessage) = await _authService.RegisterAsync(model);
			if (!success)
			{
				ModelState.AddModelError("", errorMessage ?? "Kullanıcı kaydı başarısız.");
				return View(model);
			}

			TempData["SuccessMessage"] = "Kayıt işlemi başarılı! Lütfen giriş yapınız.";
			return RedirectToAction("Login");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}

		public async Task<IActionResult> UpdatePhysicalInfo()
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login");

			var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
			var physicalInfo = await _userPhysicalInfoService.GetUserPhysicalInfoAsync(userId);

			return View(physicalInfo);
		}

		[HttpPost]
		public async Task<IActionResult> UpdatePhysicalInfo(UserPhysicalInfoDto model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var success = await _userPhysicalInfoService.UpdateUserPhysicalInfoAsync(model);
			if (!success)
			{
				ModelState.AddModelError("", "Fiziksel bilgileri güncellerken bir hata oluştu.");
				return View(model);
			}

			var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
			var updatedInfo = await _userPhysicalInfoService.GetUserPhysicalInfoAsync(userId);
			
			TempData["UserHeight"] = updatedInfo.Height.ToString();
			TempData["UserWeight"] = updatedInfo.Weight.ToString();
			TempData["SuccessMessage"] = "Fiziksel bilgileriniz başarıyla güncellendi.";
			return RedirectToAction("Index", "Category");
		}
	}
}
