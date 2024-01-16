using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebBanHangContext _context;

        public HomeController(WebBanHangContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var x = _context.GetType().Name;
            var products = _context.Products.Include(p => p.Menu);
            
            return View(await products.ToListAsync());
        }
    }
}
