using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebsite.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        public string ProductName { get; set; } = "";

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int StockQuantity { get; set; }
        public int? DiscountId { get; set; }
        public IEnumerable<SelectListItem>? Discounts { get; set; }
        public string? ImageUrl { get; set; }

        // Dùng để nhận file ảnh upload lên
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int? CategoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        public int? AuthorId { get; set; }
        public IEnumerable<SelectListItem>? Authors { get; set; }
    }
}