using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Data;
using ProjectWebsite.Models;
using ProjectWebsite.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Lấy 10 sản phẩm mới nhất
                NewestProducts = await _context.Products
                    .Include(p => p.Discount) // Lấy kèm thông tin khuyến mãi
                    .OrderByDescending(p => p.ProductId)
                    .Take(10)
                    .ToListAsync(),

                // Lấy 10 sản phẩm đang có khuyến mãi
                DiscountedProducts = await _context.Products
                    .Include(p => p.Discount) // Lấy kèm thông tin khuyến mãi
                    .Where(p => p.DiscountId != null)
                    .OrderByDescending(p => p.ProductId)
                    .Take(10)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}