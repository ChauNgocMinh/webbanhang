#nullable disable

namespace WebBanHang.ViewModels
{
    public class BuyNowVM
    {
        public Guid productId { get; set; }
        public int qty { get; set; }
        public double price { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double? TotalAmount { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
    }
}
