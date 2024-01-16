using WebBanHang.Models;

namespace WebBanHang.ViewModels
{
    public class OrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public Guid InfoOrderId { get; set; }
        public InfoOrder InfoOrder { get; set; } = null!;
    }
}
