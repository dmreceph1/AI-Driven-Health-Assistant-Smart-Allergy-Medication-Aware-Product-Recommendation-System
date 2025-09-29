using System.Collections.Generic;
using System.Threading.Tasks;
using WebUI.Models;

namespace WebUI.Services
{
    public interface IProductService
    {
        Task<(int? productId, string productName)> GetProductIdByNameAsync(string productName);
        Task<List<string>> GetProductContentsByIdAsync(int productId);
    }
}