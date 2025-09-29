using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize]
    public class UserAllergyController : Controller
    {
        private readonly IUserAllergyService _userAllergyService;

        public UserAllergyController(IUserAllergyService userAllergyService)
        {
            _userAllergyService = userAllergyService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0");
            var allergies = await _userAllergyService.GetUserAllergyAsync(userId);
            
            const int pageSize = 7;
            var paginatedAllergies = new PaginationModel<UserAllergyModel>(allergies, page, pageSize);
            
            return View(paginatedAllergies);
        }
        public async Task<IActionResult> Ekle()
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var allergies = await _userAllergyService.GetAllAllergiesAsync();
            ViewBag.Allergies = allergies;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Ekle(int allergyId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0");
            var result = await _userAllergyService.AddUserAllergyAsync(userId, allergyId);

            if (result)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Ekle");
        }
        [HttpPost]
        public async Task<IActionResult> Bitir(int userAllergyId)
        {
            var result = await _userAllergyService.FinishUserAllergyAsync(userAllergyId);
            return RedirectToAction("Index");
        }
    }
}
