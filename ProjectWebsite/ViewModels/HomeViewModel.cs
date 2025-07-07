using ProjectWebsite.Models;
using System.Collections.Generic;

namespace ProjectWebsite.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> NewestProducts { get; set; } = new List<Product>();
        public IEnumerable<Product> DiscountedProducts { get; set; } = new List<Product>();
    }
}