using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
//using WebBanHang.Areas.Identity.Data;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers;


public class OrdersController : Controller
{
    private readonly WebBanHangContext _context;
    private readonly UserManager<AppUser> _userManager;

    public OrdersController(WebBanHangContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Khách Hàng")]
    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.ToListAsync();
        var orderDetailsList = new List<List<OrderDetail>>();
        var ordersWithDetails = new List<Tuple<Order, List<OrderDetail>>>();

        foreach (var item in orders)
        {
            var orderDetails = await _context.OrderDetails
                .Where(x => x.OrderId == item.Id && x.Email == _userManager.GetUserName(User))
                .Include(o => o.Product)
                .ToListAsync();

            if (orderDetails.Count != 0)
            {
                orderDetailsList.Add(orderDetails);
                ordersWithDetails.Add(Tuple.Create(item, orderDetails));
            }
        }

        if (ordersWithDetails.Count != 0)
        {
            return View(ordersWithDetails);
        }

        return View();
    }

    public async Task<IActionResult> ManageOrder()
    {
        var orders = await _context.Orders.ToListAsync();
        var orderDetailsList = new List<List<OrderDetail>>();

        foreach (var item in orders)
        {
            var orderDetails = await _context.OrderDetails
                .Where(x => x.OrderId == item.Id)
                .Include(o => o.Product)
                .ToListAsync();

            orderDetailsList.Add(orderDetails);
        }
        var data = Tuple.Create(orders, orderDetailsList);
        return View(data);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    //public async Task<IActionResult> TemItemCart(Guid Id)
    //{
    //    var itemOrderDetails = await _context.OrderDetails.Where(x => x.Id == Id).Include(o => o.Order).Include(o => o.Product).ToListAsync();
    //    return View(itemOrderDetails);
    //}

    [HttpGet]
    public IActionResult BuyNow(Guid productId, int qty, double price)
    {
        var model = new BuyNowVM()
        {
            productId = productId,
            qty = qty,
            price = price
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BuyNow(BuyNowVM product)
    {

        var order = new Order();
        order.Id = Guid.NewGuid();
        order.PhoneNumber = product.PhoneNumber;
        order.Address = product.Address;
        order.TotalAmount = product.qty * product.price;
        order.Status = 0;
        order.Note = product.Note;
        order.Created = DateTime.Now;

        _context.Add(order);
        await _context.SaveChangesAsync();

        var orderDetail = new OrderDetail
        {
            Id = Guid.NewGuid(),
            ProductId = product.productId,
            Status = false,
            OrderId = order.Id,
            Qty = product.qty,
            ProductPrice = product.qty * product.price,
            Email = _userManager.GetUserName(User),
        };

        _context.Add(orderDetail);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    // GET: Orders/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PhoneNumber,Address,Status,Note")] Order order)
    {
        var ListItem = await _context.OrderDetails.Where(x => x.Email == _userManager.GetUserName(User) && x.Status == true).ToListAsync();
        double? TotalAmount = 0;
        foreach (var item in ListItem)
        {
            TotalAmount += item.ProductPrice;
        }

        order.Id = Guid.NewGuid();
        order.Created = DateTime.Now;
        order.TotalAmount = TotalAmount;
        _context.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in ListItem)
        {
            item.OrderId = order.Id;
            item.Status = false;
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Orders/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    // POST: Orders/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,CustomerCode,PhoneNumber,Address,TotalAmount,Status,Note,Created")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: Orders/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Orders == null)
        {
            return Problem("Entity set 'WebBanHangContext.Orders'  is null.");
        }
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(Guid id)
    {
        return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
    }


}
