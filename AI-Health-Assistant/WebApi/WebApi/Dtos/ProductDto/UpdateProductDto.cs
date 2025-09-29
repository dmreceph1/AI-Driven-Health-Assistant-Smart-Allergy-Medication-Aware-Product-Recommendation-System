namespace WebApi.Dtos.ProductDto
{
    public class UpdateProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string BarcodeNo { get; set; }
        public float Price { get; set; }
        public int CategoryID { get; set; }
    }
}
