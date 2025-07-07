using Microsoft.AspNetCore.Mvc;
using ProjectWebsite.Models;
using System.Collections.Generic;

namespace ProjectWebsite.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Bộ SGK Lớp 12 - Chân Trời Sáng Tạo", Author = "Nhiều tác giả", Price = 244000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/anh-chup-man-hinh-2024-12-02-145618-1626.png" },
                new Book { Id = 2, Title = "Bộ SGK Lớp 11 - Kết Nối Tri Thức", Author = "Nhiều tác giả", Price = 250000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/anh-chup-man-hinh-2024-12-02-103908-1327.png" },
                new Book { Id = 3, Title = "Bộ SGK Lớp 8 - Cánh Diều", Author = "Nhiều tác giả", Price = 235000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/5-5398.png" },
                new Book { Id = 4, Title = "Harry Potter và Hòn Đá Phù Thủy", Author = "J.K. Rowling", Price = 150000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/nanoflare1000z-2918.jpg" },
                new Book { Id = 5, Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", Price = 99000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/snapedit1734231237217-3821.jpg" },
                new Book { Id = 6, Title = "Nhà Giả Kim", Author = "Paulo Coelho", Price = 79000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/anh-chup-man-hinh-2024-12-10-093845-8411.png" },
                new Book { Id = 7, Title = "Cafe Cùng Tony", Author = "Tony Buổi Sáng", Price = 85000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/snapedit1726559225614-5994.jpg" },
                new Book { Id = 8, Title = "Dế Mèn Phiêu Lưu Ký", Author = "Tô Hoài", Price = 60000m, ImageUrl = "https://senhongp.vn/thumbs/185x185x1/upload/product/anh-chup-man-hinh-2024-11-05-144700-2593.png" }
            };

            return View(books);
        }
    }
}