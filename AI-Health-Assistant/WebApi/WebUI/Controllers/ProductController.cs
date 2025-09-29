using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUI.Models;
using WebUI.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Search()
        {
            ViewBag.UserName = User.Identity.Name;
            return View(new ProductSearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(ProductSearchModel model)
        {
            ModelState.Remove("RealProductName");
            
            if (!string.IsNullOrEmpty(model.ProductName))
            {
                var (productId, realProductName) = await _productService.GetProductIdByNameAsync(model.ProductName);
                
                if (productId.HasValue)
                {
                    model.RealProductName = realProductName ?? model.ProductName; 
                    model.ProductContents = await _productService.GetProductContentsByIdAsync(productId.Value);
                }
                else
                {
                    ModelState.AddModelError("", "Ürün bulunamadı.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen bir ürün adı girin.");
            }

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }
    }
}