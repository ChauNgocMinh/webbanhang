using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly WebBanHangContext _context;

        public MenuViewComponent(WebBanHangContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return _context.Menus != null ?
                        View(await _context.Menus.ToListAsync()) :
                        Content("Entity set 'WebBanHangContext.Menus'  is null.");
        }
    }
}
