#nullable disable

namespace WebBanHang.ViewModels
{
    public class OrderViewModel
    {
        public InfoOrderViewModel InfoOrder { get; set; }
        public List<OrderItemViewModel> OrderItem { get; set; }
    }
}
