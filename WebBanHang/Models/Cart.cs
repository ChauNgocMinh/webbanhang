namespace WebBanHang.Models;

public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<CartItem> CartItems { get; set; } = new();
}