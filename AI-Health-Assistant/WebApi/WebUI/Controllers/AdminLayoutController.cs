using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebUI.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminLayoutController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}