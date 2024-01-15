using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using WebBanHang.Areas.Identity.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers;

[Authorize(Roles = "Khách Hàng")]
public class OrderDetailsController : Controller
{
    private readonly WebBanHangContext _context;
    private readonly UserManager<AppUser> _userManager;
    public OrderDetailsController(WebBanHangContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: OrderDetails
    public async Task<IActionResult> Index()
    {
        var itemOrderDetails = await _context.OrderDetails.Where(x => x.Status == true && x.Email == _userManager.GetUserName(User)).Include(o => o.Order).Include(o => o.Product).ToListAsync();
        return View(itemOrderDetails);
    }

    // GET: OrderDetails/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.OrderDetails == null)
        {
            return NotFound();
        }

        var orderDetail = await _context.OrderDetails
            .Include(o => o.Order)
            .Include(o => o.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderDetail == null)
        {
            return NotFound();
        }

        return View(orderDetail);
    }


    [HttpGet]
    public async Task<IActionResult> Create(Guid productId, int qty)
    {
        var itemOrderDetail = _context.OrderDetails.Where(x => x.ProductId == productId && x.Status == true && x.Email == _userManager.GetUserName(User)).FirstOrDefault();
        var itemProduct = _context.Products.Where(x => x.Id == productId).FirstOrDefault();
        OrderDetail orderDetail = new OrderDetail();
        if (itemOrderDetail != null)
        {
            itemOrderDetail.Qty += qty;
            itemOrderDetail.ProductPrice = itemOrderDetail.Qty * itemProduct.Price;

            _context.Update(itemOrderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            orderDetail = new OrderDetail
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Status = true,
                OrderId = null,
                Qty = qty,
                ProductPrice = qty * itemProduct.Price,
                Email = _userManager.GetUserName(User),
            };

            _context.Add(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: OrderDetails/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.OrderDetails == null)
        {
            return NotFound();
        }

        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail == null)
        {
            return NotFound();
        }
        ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
        return View(orderDetail);
    }

    // POST: OrderDetails/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,OrderId,ProductId,ProductPrice,Qty,Status")] OrderDetail orderDetail)
    {
        if (id != orderDetail.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(orderDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(orderDetail.Id))
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
        ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
        return View(orderDetail);
    }


    // POST: OrderDetails/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (_context.OrderDetails == null)
        {
            return Problem("Entity set 'WebBanHangContext.OrderDetails'  is null.");
        }
        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail != null)
        {
            _context.OrderDetails.Remove(orderDetail);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderDetailExists(Guid id)
    {
      return (_context.OrderDetails?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
