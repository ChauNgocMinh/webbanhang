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
        var cart = new Cart();

        if (cartViewModel.Id != null && cartViewModel.Id != Guid.Empty)
        {
            cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartViewModel.Id) ?? new Cart();
        }

        var cartItems = cartViewModel.CartItems.Select(cvm => new CartItem
        {
            Price = cvm.Price,
            Quantity = cvm.Quantity,
            ProductId = cvm.ProductId,
            ProductName = cvm.ProductName,
            Image = cvm.Image,
        }).ToList();

        if (cartItems.Count > 0)
        {
            cart.CartItems.AddRange(cartItems);
        }

        _context.Add(cart);

        await _context.SaveChangesAsync();

        return Ok(cart.Id);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}