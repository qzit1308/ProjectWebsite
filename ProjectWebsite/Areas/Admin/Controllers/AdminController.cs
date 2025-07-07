using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Areas.Admin.ViewModels;
using ProjectWebsite.Data;
using ProjectWebsite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- CÁC TRANG QUẢN LÝ CHÍNH ---
        public IActionResult Index() => View();
        public async Task<IActionResult> Users() => View(await _context.Users.Include(u => u.Role).ToListAsync());
        public async Task<IActionResult> Orders() => View(await _context.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).ToListAsync());
        public async Task<IActionResult> Settings() => View(await _context.Settings.ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue));
        public async Task<IActionResult> Discounts() => View(await _context.Discounts.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Settings(Dictionary<string, string> settings)
        {
            foreach (var (key, value) in settings)
            {
                var settingToUpdate = await _context.Settings.FindAsync(key);
                if (settingToUpdate != null && settingToUpdate.SettingValue != value)
                {
                    settingToUpdate.SettingValue = value;
                }
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật cài đặt thành công!";
            return RedirectToAction("Settings");
        }

        // --- QUẢN LÝ KHUYẾN MÃI ---
        public IActionResult CreateDiscount() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(Discount discount)
        {
            if (ModelState.IsValid)
            {
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo khuyến mãi mới thành công!";
                return RedirectToAction("Discounts");
            }
            return View(discount);
        }

        // --- QUẢN LÝ SÁCH (CRUD) ---
        public async Task<IActionResult> Products()
        {
            var pageViewModel = new ProductsPageViewModel
            {
                Products = await _context.Products.Include(p => p.Category).Include(p => p.Author).OrderByDescending(p => p.ProductId).ToListAsync(),
                NewProduct = new ProductViewModel
                {
                    Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToListAsync(),
                    Authors = await _context.Authors.Select(a => new SelectListItem { Value = a.AuthorId.ToString(), Text = a.AuthorName }).ToListAsync(),
                    Discounts = await _context.Discounts.Select(d => new SelectListItem { Value = d.DiscountId.ToString(), Text = d.DiscountName }).ToListAsync()
                }
            };
            return View(pageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductsPageViewModel pageViewModel)
        {
            var viewModel = pageViewModel.NewProduct;
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = viewModel.ProductName,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    CategoryId = viewModel.CategoryId,
                    AuthorId = viewModel.AuthorId,
                    DiscountId = viewModel.DiscountId
                };
                if (viewModel.ImageFile != null)
                {
                    product.ImageUrl = await UploadImage(viewModel.ImageFile);
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm sách mới thành công!";
            }
            else
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Thêm thất bại! Lỗi: {errors}";
            }
            return RedirectToAction("Products");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                AuthorId = product.AuthorId,
                DiscountId = product.DiscountId,
                Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToListAsync(),
                Authors = await _context.Authors.Select(a => new SelectListItem { Value = a.AuthorId.ToString(), Text = a.AuthorName }).ToListAsync(),
                Discounts = await _context.Discounts.Select(d => new SelectListItem { Value = d.DiscountId.ToString(), Text = d.DiscountName }).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var productToUpdate = await _context.Products.FindAsync(viewModel.ProductId);
                if (productToUpdate == null) return NotFound();

                if (viewModel.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(productToUpdate.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToUpdate.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }
                    productToUpdate.ImageUrl = await UploadImage(viewModel.ImageFile);
                }

                productToUpdate.ProductName = viewModel.ProductName;
                productToUpdate.Description = viewModel.Description;
                productToUpdate.Price = viewModel.Price;
                productToUpdate.StockQuantity = viewModel.StockQuantity;
                productToUpdate.CategoryId = viewModel.CategoryId;
                productToUpdate.AuthorId = viewModel.AuthorId;
                productToUpdate.DiscountId = viewModel.DiscountId;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Products");
            }
            viewModel.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToListAsync();
            viewModel.Authors = await _context.Authors.Select(a => new SelectListItem { Value = a.AuthorId.ToString(), Text = a.AuthorName }).ToListAsync();
            viewModel.Discounts = await _context.Discounts.Select(d => new SelectListItem { Value = d.DiscountId.ToString(), Text = d.DiscountName }).ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath)) { System.IO.File.Delete(imagePath); }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công sản phẩm ID {id}.";
            return Ok();
        }

        // --- XÁC NHẬN ĐƠN HÀNG THỦ CÔNG ---
        [HttpGet]
        public IActionResult ConfirmOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng với ID này.";
                return View();
            }
            if (order.Status == "Pending")
            {
                order.Status = "Processing";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã xác nhận thanh toán thành công cho đơn hàng ID: {orderId}";
            }
            else
            {
                TempData["WarningMessage"] = $"Đơn hàng ID: {orderId} đã ở trạng thái '{order.Status}', không thể xác nhận lại.";
            }
            return RedirectToAction("Orders");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công đơn hàng ID {id}.";
            return Ok();
        }

        // HÀM HELPER UPLOAD ẢNH
        private async Task<string?> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return $"/images/products/{uniqueFileName}";
        }
    }
}