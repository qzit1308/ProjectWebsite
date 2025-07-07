using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ĐĂNG KÝ ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email này đã được sử dụng.");
                    return View(model);
                }

                var customerRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "Customer");
                if (customerRole == null)
                {
                    ModelState.AddModelError(string.Empty, "Lỗi hệ thống: Không tìm thấy vai trò người dùng.");
                    return View(model);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Role = customerRole
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // --- ĐĂNG NHẬP ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                                        .Include(u => u.Role)
                                        .SingleOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.RoleName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // --- BẮT ĐẦU CODE HỢP NHẤT GIỎ HÀNG ---
                    var sessionCart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartController.CART_KEY);
                    if (sessionCart != null && sessionCart.Any())
                    {
                        var userId = user.UserId;
                        foreach (var item in sessionCart)
                        {
                            var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == item.ProductId);
                            if (dbCartItem != null)
                            {
                                dbCartItem.Quantity += item.Quantity; // Nếu đã có, cộng dồn số lượng
                            }
                            else
                            {
                                _context.CartItems.Add(new CartItem { UserId = userId, ProductId = item.ProductId, Quantity = item.Quantity });
                            }
                        }
                        await _context.SaveChangesAsync();
                        // Xóa giỏ hàng trong session sau khi đã chuyển vào DB
                        HttpContext.Session.Remove(CartController.CART_KEY);
                    }
                    // --- KẾT THÚC CODE HỢP NHẤT GIỎ HÀNG ---

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        // --- ĐĂNG XUẤT ---
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}