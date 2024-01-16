namespace WebBanHang.Models;

public class DetailColor
{
    public Guid Id { get; set; }
    public double Price { get; set; } = 0;
    public Guid? IdColor { get; set; }
    public Guid? IdProduct { get; set; }
    public virtual Product? IdProductNavigation { get; set; }
    public virtual Color? IdColorNavigation { get; set; }
}
