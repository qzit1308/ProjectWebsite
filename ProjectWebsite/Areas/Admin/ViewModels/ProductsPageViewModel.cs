using ProjectWebsite.Models;

namespace ProjectWebsite.Areas.Admin.ViewModels
{
    public class ProductsPageViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public ProductViewModel NewProduct { get; set; } = new ProductViewModel();
    }
}