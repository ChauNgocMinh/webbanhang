using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Identity.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class CustomerController : Controller
    {
        private readonly WebBanHangContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly UserManager<WebBanHangUser> _userManager;

        public CustomerController(WebBanHangContext context, IWebHostEnvironment webHost, UserManager<WebBanHangUser> userManager)
        {
            _context = context;
            _webHost = webHost;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Menu).ToListAsync();
            var menus = await _context.Menus.ToListAsync();
            var model = new Tuple<IEnumerable<Product>, IEnumerable<Menu>>(products, menus);

            return View(model);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Menu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> GetByMenu(string id)
        {
            ViewBag.MenuId = id;
            var products = await _context.Products
                .Include(p => p.Menu)
                .Where(p => p.Menu.Id.ToString() == id)
                .ToListAsync();
            var menus = await _context.Menus.ToListAsync();
            var model = new Tuple<IEnumerable<Product>, IEnumerable<Menu>>(products, menus);
            return View(model);
        }
        //public async Task<IActionResult> AddItemToCart(Guid id, int? quantityNumber)
        //{
        //    if (Session["cart"] != null)
        //    {

        //    }

        //}
    }
}
