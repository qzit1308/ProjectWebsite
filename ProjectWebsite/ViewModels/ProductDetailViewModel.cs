using ProjectWebsite.Models;
using System.Collections.Generic;

namespace ProjectWebsite.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = new();
        public IEnumerable<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}