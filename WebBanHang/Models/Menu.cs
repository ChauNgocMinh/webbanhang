namespace WebBanHang.Models;

public class Menu
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public List<Product> Products { get; set; } = new();
}
