using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers;

public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly WebBanHangContext _context;

    public CartController(ILogger<CartController> logger, WebBanHangContext context)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? Id)
    {
        if (Id == null) return View(new Cart());

        var cart = await _context.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == Id) ?? new Cart();

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(CartViewModel cartViewModel)
    {
        if (cartViewModel.CartItems == null || cartViewModel.CartItems.Count == 0)
        {
            return BadRequest("CartItems cannot be empty.");
        }

        var cart = new Cart();

        if (cartViewModel.Id != null && cartViewModel.Id != Guid.Empty)
        {
            cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartViewModel.Id) ?? new Cart();
        }

        foreach (var cartItemViewModel in cartViewModel.CartItems)
        {
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductName == cartItemViewModel.ProductName);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                existingCartItem.Price += cartItemViewModel.Price;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    Price = cartItemViewModel.Price,
                    Quantity = 1,
                    ProductId = cartItemViewModel.ProductId,
                    ProductName = cartItemViewModel.ProductName,
                    Image = cartItemViewModel.Image,
                };

                cart.CartItems.Add(newCartItem);
            }
        }
        if (cartViewModel.Id == null || cartViewModel.Id == Guid.Empty)
        {
            _context.Add(cart);
        }

        await _context.SaveChangesAsync();

        return Ok(cart.Id);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}