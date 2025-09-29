using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.ProductDto;
using WebApi.Repositories.ProductRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var x = await _productRepository.GetAllProductAsync();
            return Ok(x);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            _productRepository.CreateProduct(createProductDto);
            return Ok("Başarılı");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _productRepository.DeleteProduct(id);
            return Ok("başarılı");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            _productRepository.UpdateProduct(updateProductDto);
            return Ok("başarılı");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var x = await _productRepository.GetProduct(id);
            return Ok(x);
        }

        [HttpGet("getByBarcode")]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            var product = await _productRepository.GetProductByBarcodeAsync(barcode);

            if (product == null)
                return NotFound("Barkoda ait ürün bulunamadı.");

            return Ok(product);
        }

        [HttpGet("getByName")]
        public async Task<IActionResult> GetProductByName(string productName)
        {
            var product = await _productRepository.GetProductByNameAsync(productName);

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            return Ok(product);
        }

        [HttpGet("getProductIdByName")]
        public async Task<IActionResult> GetProductIdByName(string productName)
        {
            var product = await _productRepository.GetProductByNameAsync(productName);

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            return Ok(new { productId = product.ProductID, productName = product.ProductName });
        }

    }
}
