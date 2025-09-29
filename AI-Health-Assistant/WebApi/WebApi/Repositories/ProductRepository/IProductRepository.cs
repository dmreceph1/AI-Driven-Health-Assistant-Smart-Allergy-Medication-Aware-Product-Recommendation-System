using WebApi.Dtos.ProductDto;

namespace WebApi.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<List<ResultProductDto>> GetAllProductAsync();
        void CreateProduct(CreateProductDto productDto);
        void DeleteProduct(int id);
        void UpdateProduct(UpdateProductDto updateProductDto);
        Task<GetByIDProductDto> GetProduct(int id);
        Task<GetByIDProductDto> GetProductByBarcodeAsync(string barcode);
        Task<GetByIDProductDto> GetProductByNameAsync(string productName);
        Task<int?> GetProductIdByNameAsync(string productName);

    }
}
