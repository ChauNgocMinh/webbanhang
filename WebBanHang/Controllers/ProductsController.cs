using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers;

public class ProductsController : Controller
{
    private readonly WebBanHangContext _context;
    private readonly IWebHostEnvironment _webHost;

    public ProductsController(WebBanHangContext context, IWebHostEnvironment webHost)
    {
        _context = context;
        _webHost = webHost;
    }

    // GET: Products
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = _context.Products.Include(p => p.Menu);
        return View(await products.ToListAsync());
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(Guid? id, [FromQuery] Guid? IdRom)
    {
        if (id == null || _context.Products == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Menu)
            .Include(p => p.DetailRoms)
            .ThenInclude(dr => dr.IdRomNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.IdRom = IdRom;
        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        ViewData["MenuName"] = new SelectList(_context.Menus, "Id", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + '_' + product.ImageFile.FileName;
                product.Image = uniqueFileName;
                string fileSavePath = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image);
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }
            }
            product.Created = DateTime.Now;
            product.Id = Guid.NewGuid();
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["MenuId"] = new SelectList(_context.Menus, "Id", "Id", product.MenuId);
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Products == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }
        ViewData["MenuName"] = new SelectList(_context.Menus, "Id", "Name", product.MenuId);
        return View(product);
    }

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, Product model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                //nếu chọn ảnh khác thì cập nhật ảnh
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
                    //xóa ảnh cũ 
                    var oldImage = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image);
                    if(System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                    //thêm ảnh mới
                    string uniqueFileName = Guid.NewGuid().ToString() + '_' + model.ImageFile.FileName;
                    product.Image = uniqueFileName;
                    string fileSavePath = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image);
                    using (var stream = new FileStream(fileSavePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    _context.Update(product);
                }
                //cạp nhật bình thường 
                else
                {
                    _context.Update(model);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(model.Id))
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
        ViewData["MenuName"] = new SelectList(_context.Menus, "Id", "Name", model.Name);
        return View(model);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
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

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Products == null)
        {
            return Problem("Entity set 'WebBanHangContext.Products'  is null.");
        }
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(Guid id)
    {
      return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    [HttpPost]
    public async Task<IActionResult> Search(string keyword)
    {
        if (!string.IsNullOrEmpty(keyword))
        {
            IQueryable<Product> data = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(u => u.Name.Contains(keyword));
            }

            return View(data);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> GetByMenu(string id)
    {
        ViewBag.MenuId = id;
        var products = _context.Products.Include(p => p.Menu)
            .Where(p => p.Menu.Id.ToString() == id);
        ViewBag.MenuName = _context.Menus.FirstOrDefault(m => m.Id.ToString() == id)?.Name;
        return View(await products.ToListAsync());
    }

    public async Task<IActionResult> SearchInMenu(string id, string keyword)
    {
        if (!string.IsNullOrEmpty(keyword))
        {
            IQueryable<Product> data = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(u =>u.Menu.Id.ToString() == id && u.Name.Contains(keyword));
            }

            return View(data);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

}
