using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Data;
using ProjectWebsite.Models;
using ProjectWebsite.Services; // Thêm using này
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectWebsite.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MailService _mailService; // Thêm service mail

        // Sửa lại constructor để nhận thêm MailService
        public CheckoutController(ApplicationDbContext context, MailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt hàng." });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId); // Lấy thông tin user để có email
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin tài khoản." });
            }

            var cartItems = await _context.CartItems
                                        .Include(ci => ci.Product)
                                        .Where(ci => ci.UserId == userId)
                                        .ToListAsync();

            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
            }

            // 1. Tạo đơn hàng mới
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                RecipientName = user.FullName ?? user.Username,
                ShippingAddress = user.Address ?? "Chưa có địa chỉ",
                RecipientPhone = user.PhoneNumber ?? "Chưa có SĐT"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 2. Thêm chi tiết đơn hàng
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail { OrderId = order.OrderId, ProductId = item.ProductId, Quantity = item.Quantity, PriceAtPurchase = item.Product.Price };
                _context.OrderDetails.Add(orderDetail);
            }

            // 3. Xóa giỏ hàng
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // --- BẮT ĐẦU GỬI MAIL CHO KHÁCH HÀNG ---
            try
            {
                var subject = $"Nhà Sách Online: Đã nhận đơn hàng #{order.OrderId}";
                var body = $"Chào {user.Username},<br><br>" +
                           $"Cảm ơn bạn đã đặt hàng. Đơn hàng <strong>#{order.OrderId}</strong> của bạn đã được chúng tôi ghi nhận với trạng thái \"Chờ thanh toán\".<br>" +
                           $"Tổng giá trị đơn hàng là: <strong>{order.TotalAmount:N0}đ</strong>.<br><br>" +
                           "Vui lòng hoàn tất thanh toán để chúng tôi có thể xử lý đơn hàng sớm nhất.<br><br>" +
                           "Trân trọng,<br>Nhà Sách Online.";
                await _mailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi nếu gửi mail thất bại, nhưng không làm dừng quy trình
                Console.WriteLine($"Gửi mail cho khách hàng thất bại: {ex.Message}");
            }
            // --- KẾT THÚC GỬI MAIL ---

            return Json(new { success = true, orderId = order.OrderId, amount = order.TotalAmount, userId = userId });
        }
    }
}