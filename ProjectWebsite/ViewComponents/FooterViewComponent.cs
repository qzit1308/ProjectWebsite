using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Data;

namespace ProjectWebsite.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FooterViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy tất cả các cài đặt và chuyển thành một Dictionary để dễ sử dụng
            var settings = await _context.Settings
                                         .ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);

            // Gửi dictionary này tới View của component
            return View(settings);
        }
    }
}