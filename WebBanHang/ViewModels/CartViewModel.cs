namespace WebBanHang.ViewModels;

public class CartViewModel
{
    public Guid? Id { get; set; }
    public List<CartItemViewModel> CartItems { get; set; } = new();
}