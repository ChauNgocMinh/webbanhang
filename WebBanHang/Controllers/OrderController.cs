using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly WebBanHangContext _context;
        private readonly PaymentServices _paymentServices;

        public OrderController(ILogger<CartController> logger, WebBanHangContext context, IHttpContextAccessor httpContextAccessor, PaymentServices paymentServices)
        {
            _context = context;
            _logger = logger;
            _paymentServices = paymentServices;
        }

        [HttpPost]
        public async Task<IActionResult> ProceedPayment(int paymentType, OrderViewModel model, Guid cartId)
        {
            if (paymentType == -1)
            {
                return BadRequest("Payment not selected");
            }

            var infoOrder = new InfoOrder
            {
                Status = 1,
                PaymentMethod = model.InfoOrder.PaymentMethod,
                Name = model.InfoOrder.Name,
                Phone = model.InfoOrder.Phone,
                Email = model.InfoOrder.Email,
                City = model.InfoOrder.City,
                District = model.InfoOrder.District,
                Address = model.InfoOrder.Address,
                Total = model.InfoOrder.Total,
                Date = DateTime.Now,
                OrderItems = model.OrderItem.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductName = item.ProductName,
                    Image = item.Image,
                }).ToList()
            };

            _context.InfoOrders.Add(infoOrder);

            await _context.SaveChangesAsync();

            if (paymentType == 1)
            {
                return Ok(_paymentServices.CreateVNPAYRequestURL(
                    model,
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}{Url.Action("AfterPayment", "Order", new { cartId })!}",
                    "127.0.0.1",
                    "NCB",
                    Guid.NewGuid()
                    )
                );
            }

            return Ok(_paymentServices.CreatePayPalRequestURL(HttpContext, model));
        }

        [HttpGet]
        public async Task<IActionResult> AfterPayment(Guid cartId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                _context.Remove(cart);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var order = await _context.InfoOrders.Select(o => new InfoOrder
            {
                Id = o.Id,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                Name = o.Name,
                Phone = o.Phone,
                Email = o.Email,
                City = o.City,
                District = o.District,
                Address = o.Address,
                Total = (float?)o.Total,
                Date = o.Date,
                OrderItems = o.OrderItems
            }).ToListAsync();
            return View(order);
        }
    }
}
