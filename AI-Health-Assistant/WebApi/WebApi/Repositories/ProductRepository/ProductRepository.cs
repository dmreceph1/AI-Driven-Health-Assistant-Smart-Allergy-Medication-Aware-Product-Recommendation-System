using Dapper;
using WebApi.Dtos.ProductDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;

        public ProductRepository(Context context)
        {
            _context = context;
        }

        public async void CreateProduct(CreateProductDto productDto)
        {
            string query = "insert into Product (ProductName, ImageUrl, BarcodeNo, Price, CategoryID) Values (@productName, @ımageUrl, @barcodeNo, @price, @c)";
            var parameters = new DynamicParameters();
            parameters.Add("@productName", productDto.ProductName);
            parameters.Add("@ımageUrl", productDto.ImageUrl);
            parameters.Add("@barcodeNo", productDto.BarcodeNo);
            parameters.Add("@price", productDto.Price);
            parameters.Add("@categoryID", productDto.CategoryID);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteProduct(int id)
        {
            string query = "Delete From Product Where ProductID=@productID";
            var parameters = new DynamicParameters();
            parameters.Add("@productID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultProductDto>> GetAllProductAsync()
        {
            string query = "Select*From Product";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultProductDto>(query);
                return x.ToList();
            }
        }

        public async Task<GetByIDProductDto> GetProduct(int id)
        {
            string query = "Select * from Product Where ProductID=@productID";
            var parameters = new DynamicParameters();
            parameters.Add("@productID", id);
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryFirstOrDefaultAsync<GetByIDProductDto>(query, parameters);
                return x;
            }
        }

        public async void UpdateProduct(UpdateProductDto updateProductDto)
        {
            string query = "Update product Set ProductName=@productName,ImageUrl=@ımageUrl,BarcodeNo=@barcodeNo,Price=@price,CategoryID=@categoryID Where ProductID=@productID";
            var parameters = new DynamicParameters();
            parameters.Add("@productName", updateProductDto.ProductName);
            parameters.Add("@ımageUrl", updateProductDto.ImageUrl);
            parameters.Add("@barcodeNo", updateProductDto.BarcodeNo);
            parameters.Add("@price", updateProductDto.Price);
            parameters.Add("@categoryID", updateProductDto.CategoryID);
            parameters.Add("@productID", updateProductDto.ProductID);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<GetByIDProductDto> GetProductByBarcodeAsync(string barcode)
        {
            string query = "SELECT * FROM Product WHERE BarcodeNo = @barcode";
            var parameters = new DynamicParameters();
            parameters.Add("@barcode", barcode);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetByIDProductDto>(query, parameters);
                return result;
            }
        }

        public async Task<GetByIDProductDto> GetProductByNameAsync(string productName)
        {
            string query = "SELECT * FROM Product WHERE ProductName LIKE @productName";
            var parameters = new DynamicParameters();
            parameters.Add("@productName", $"%{productName}%"); // benzer eşleşme yapabilmeme yarar

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetByIDProductDto>(query, parameters);
                return result;
            }
        }

        public async Task<int?> GetProductIdByNameAsync(string productName)
        {
            string query = "SELECT ProductID, ProductName FROM Product WHERE ProductName LIKE @productName";
            var parameters = new DynamicParameters();
            parameters.Add("@productName", $"%{productName}%");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);
                return result?.ProductID;
            }
        }
    }
}
