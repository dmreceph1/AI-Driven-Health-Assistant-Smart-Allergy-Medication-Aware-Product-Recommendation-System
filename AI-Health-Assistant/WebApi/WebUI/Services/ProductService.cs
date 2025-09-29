using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebUI.Models;

namespace WebUI.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7222/api/");
        }

        public async Task<(int? productId, string productName)> GetProductIdByNameAsync(string productName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Products/getProductIdByName?productName={productName}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ProductIdResponse>();
                    return (result?.ProductId, result?.ProductName);
                }
                
                return (null, null);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        public async Task<List<string>> GetProductContentsByIdAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ProductContents/GetContentsByProductId?productId={productId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<string>>();
                }
                
                return new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }

    public class ProductIdResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}