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

        public OrderController(ILogger<CartController> logger, WebBanHangContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Buy(OrderViewModel orderViewModel, Guid CartId)
        {
            var infoOrder = new InfoOrder
            {
                Status = 1,
                Id = Guid.NewGuid(),
                PaymentMethod = orderViewModel.InfoOrder.PaymentMethod,
                Name = orderViewModel.InfoOrder.Name,
                Phone = orderViewModel.InfoOrder.Phone,
                Email = orderViewModel.InfoOrder.Email,
                City = orderViewModel.InfoOrder.City,
                District = orderViewModel.InfoOrder.District,
                Address = orderViewModel.InfoOrder.Address,
                Total = orderViewModel.InfoOrder.Total,
                Date = DateTime.Now,
            };
            _context.InfoOrders.Add(infoOrder);
            await _context.SaveChangesAsync();
            foreach (var item in orderViewModel.OrderItem)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductName = item.ProductName,
                    Image = item.Image,
                    InfoOrderId = infoOrder.Id,
                };
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
            }

            var cartItem = _context.CartItems.Where(x=>x.CartId == CartId).ToList();
            _context.RemoveRange(cartItem);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đặt hàng thành công" });
        }
    }
}
