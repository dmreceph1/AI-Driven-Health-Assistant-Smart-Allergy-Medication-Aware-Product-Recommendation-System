using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize]
    public class UserMedicationController : Controller
    {
        private readonly IUserMedicationService _userMedicationService;

        public UserMedicationController(IUserMedicationService userMedicationService)
        {
            _userMedicationService = userMedicationService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0");
            var medications = await _userMedicationService.GetUserMedicationAsync(userId);
            
            const int pageSize = 7;
            var paginatedMedications = new PaginationModel<UserMedicationModel>(medications, page, pageSize);
            
            return View(paginatedMedications);
        }

        public async Task<IActionResult> Ekle()
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var medications = await _userMedicationService.GetAllMedicationsAsync();
            ViewBag.Medications = medications;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(int medicationId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0");
            var result = await _userMedicationService.AddUserMedicationAsync(userId, medicationId);
            
            if (result)
            {
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Ekle");
        }

        [HttpPost]
        public async Task<IActionResult> Bitir(int userMedicationId)
        {
            var result = await _userMedicationService.FinishUserMedicationAsync(userMedicationId);
            return RedirectToAction("Index");
        }
    }
}
