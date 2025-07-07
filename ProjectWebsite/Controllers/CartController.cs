using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Data;
using ProjectWebsite.Helpers;
using ProjectWebsite.Models;
using ProjectWebsite.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectWebsite.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public const string CART_KEY = "shopping_cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session (chỉ dùng cho khách)
        private List<CartItemViewModel> SessionCart =>
            HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CART_KEY) ?? new List<CartItemViewModel>();

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var cartItems = new List<CartItemViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                cartItems = await _context.CartItems
                    .Where(ci => ci.UserId == userId)
                    .Select(ci => new CartItemViewModel
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.ProductName,
                        Price = ci.Product.Price,
                        ImageUrl = ci.Product.ImageUrl,
                        Quantity = ci.Quantity
                    }).ToListAsync();
            }
            else
            {
                cartItems = SessionCart;
            }

            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

                if (cartItem == null)
                {
                    _context.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });
                }
                else
                {
                    cartItem.Quantity += quantity;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                var cart = SessionCart;
                var cartItem = cart.SingleOrDefault(p => p.ProductId == productId);
                if (cartItem == null)
                {
                    cart.Add(new CartItemViewModel { ProductId = product.ProductId, ProductName = product.ProductName, Price = product.Price, ImageUrl = product.ImageUrl, Quantity = quantity });
                }
                else
                {
                    cartItem.Quantity += quantity;
                }
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng." });
        }

        // Các action RemoveFromCart, UpdateCart cũng cần được sửa tương tự
    }
}