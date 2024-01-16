namespace WebBanHang.Models;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = null!;
    public string Image { get; set; } = null!;

    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;
}