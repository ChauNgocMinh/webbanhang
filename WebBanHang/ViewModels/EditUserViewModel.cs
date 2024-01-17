namespace WebBanHang.ViewModels;

public class EditUserViewModel
{
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
