namespace WebBanHang.ViewModels;

public class CartItemViewModel
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Image { get; set; } = null!;
}