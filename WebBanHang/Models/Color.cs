namespace WebBanHang.Models;

#nullable disable

public class Color
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ColorCode { get; set; } = null!;
    public virtual ICollection<DetailColor> DetailColors { get; set; }
}
