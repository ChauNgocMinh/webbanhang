namespace WebBanHang.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public Guid? IdProduct { get; set; }
        public string? Img { get; set; }
        public virtual Product? IdProductNavigation { get; set; }
    }
}
