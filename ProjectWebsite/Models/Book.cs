namespace ProjectWebsite.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        // Thêm các thuộc tính sau
        public int DiscountPercentage { get; set; } = 0; // % giảm giá, ví dụ: 10 cho 10%
        public int Rating { get; set; } = 0; // Số sao, ví dụ: 4
        public int ReviewCount { get; set; } = 0; // Số lượt đánh giá
    }
}