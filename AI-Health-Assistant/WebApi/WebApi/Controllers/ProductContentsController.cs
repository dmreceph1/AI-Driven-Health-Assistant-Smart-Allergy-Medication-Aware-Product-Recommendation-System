using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Repositories.ProductContentsRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductContentsController : ControllerBase
    {
        private readonly IProductContentsRepository _productContentsRepository;

        public ProductContentsController(IProductContentsRepository productContentsRepository)
        {
            _productContentsRepository = productContentsRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ProductContentsList()
        {
            var x = await _productContentsRepository.GetAllProductContentAsync();
            return Ok(x);
        }
        [HttpGet("ProductContentsWithProduct")]
        public async Task<IActionResult> ProductContentsWithProduct()
        {
            var x = await _productContentsRepository.GetResultProductContentWithProductDtos();
            return Ok(x);
        }

        [HttpGet("GetContentsByProductId")]
        public async Task<IActionResult> GetContentsByProductId(int productId)
        {
            var contents = await _productContentsRepository.GetContentsByProductIdAsync(productId);
            if (contents == null || !contents.Any())
            {
                return NotFound("Ürün içeriği bulunamadı.");
            }

            return Ok(contents);
        }
    }
}
