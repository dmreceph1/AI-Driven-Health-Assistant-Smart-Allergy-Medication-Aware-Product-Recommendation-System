using Dapper;
using WebApi.Dtos.ProductContentsDto;
using WebApi.Dtos.ProductDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.ProductContentsRepository
{
    public class ProductContentsRepository : IProductContentsRepository
    {
        private readonly Context _context;

        public ProductContentsRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<ResultProductContentsDto>> GetAllProductContentAsync()
        {
            string query = "Select*From ProductContents";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultProductContentsDto>(query);
                return x.ToList();
            }
        }

        public async Task<List<string>> GetContentsByProductIdAsync(int productId)
        {
            string query = "SELECT Component FROM ProductContents WHERE ProductID = @ProductId";
            using (var connection = _context.CreateConnection())
            {
                var contents = await connection.QueryAsync<string>(query, new { ProductId = productId });
                return contents.ToList();
            }
        }

        public async Task<List<ResultProductContentWithProductDto>> GetResultProductContentWithProductDtos()
        {
            string query = "Select ContentID,Component,ProductName From ProductContents inner join Product on ProductContents.ProductID = Product.ProductID";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultProductContentWithProductDto>(query);
                return x.ToList();
            }
        }
    }
}
