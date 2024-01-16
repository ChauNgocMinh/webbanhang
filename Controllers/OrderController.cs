using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly WebBanHangContext _context;

        public OrderController(ILogger<CartController> logger, WebBanHangContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Buy(OrderViewModel orderViewModel)
        {
            return View();
        }
    }
}
