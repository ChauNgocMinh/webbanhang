namespace WebBanHang.Models;

public class DetailColor
{
    public Guid Id { get; set; }
    public double Price { get; set; } = 0;

    public Guid IdProductColor { get; set; }
    public Color ProductColor { get; set; } = null!;

    public Guid IdProduct { get; set; }
    public Product Product { get; set; } = null!;
}
