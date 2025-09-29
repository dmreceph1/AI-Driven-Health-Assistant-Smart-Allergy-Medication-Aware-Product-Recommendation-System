using System.Collections.Generic;

namespace WebUI.Models
{
    public class ProductSearchModel
    {
        public string ProductName { get; set; }
        public string RealProductName { get; set; }
        public List<string> ProductContents { get; set; } = new List<string>();
    }
}