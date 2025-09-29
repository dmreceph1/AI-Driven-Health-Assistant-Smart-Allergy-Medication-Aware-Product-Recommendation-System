using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;

namespace WebUI.Controllers
{
	[Authorize]
	public class CategoryController : Controller
    {
        private readonly IUserPhysicalInfoService _userPhysicalInfoService;

        public CategoryController(IUserPhysicalInfoService userPhysicalInfoService)
        {
            _userPhysicalInfoService = userPhysicalInfoService;
        }

        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
                var physicalInfo = await _userPhysicalInfoService.GetUserPhysicalInfoAsync(userId);
                
                ViewBag.UserHeight = physicalInfo.Height > 0 ? physicalInfo.Height : (decimal?)null;
                ViewBag.UserWeight = physicalInfo.Weight > 0 ? physicalInfo.Weight : (decimal?)null;
            }

            return View();
		}
    }
}
