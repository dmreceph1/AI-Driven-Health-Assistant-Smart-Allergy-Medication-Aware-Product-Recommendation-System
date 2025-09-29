using WebApi.Dtos.ProductContentsDto;

namespace WebApi.Repositories.ProductContentsRepository
{
    public interface IProductContentsRepository
    {
        Task<List<ResultProductContentsDto>> GetAllProductContentAsync();
        Task<List<ResultProductContentWithProductDto>> GetResultProductContentWithProductDtos();
        Task<List<string>> GetContentsByProductIdAsync(int productId);
    }
}
