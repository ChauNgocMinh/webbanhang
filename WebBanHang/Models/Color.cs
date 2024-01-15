namespace WebBanHang.Models;

public class Color
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public HashSet<DetailColor> Colors { get; set; } = new();
}
