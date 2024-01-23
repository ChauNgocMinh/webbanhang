using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly WebBanHangContext _context;
    private readonly IWebHostEnvironment _webHost;

    public ProductsController(WebBanHangContext context, IWebHostEnvironment webHost)
    {
        _context = context;
        _webHost = webHost;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = _context.Products.OrderByDescending(p => p.Created).Include(p => p.Menu);
        return View(await products.ToListAsync());
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id, [FromQuery] Guid? IdRom, [FromQuery] Guid? IdColor)
    {
        var product = await _context.Products
            .Include(p => p.Menu)
            .Include(p => p.DetailRoms)
            .ThenInclude(dr => dr.IdRomNavigation)
            .Include(x => x.DetailColors)
            .ThenInclude(xx => xx.IdColorNavigation)
            .Include(a => a.Images)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        if (IdRom == null)
        {
            var Rom = await _context.DetailRoms
                .Where(x => x.IdProduct == product.Id)
                .OrderBy(x => x.Price)
                .FirstOrDefaultAsync();
            IdRom = Rom?.IdRom;
        }

        if (IdColor == null)
        {
            var Color = await _context.DetailColors
                .Where(x => x.IdProduct == product.Id)
                .OrderBy(x => x.Price)
                .FirstOrDefaultAsync();
            IdColor = Color?.IdColor;
        }
        var newProducts = await _context.Products
                                        .Where(x => x.MenuId == product.MenuId)
                                        .OrderByDescending(p => p.Created)
                                        .Take(4)
                                        .ToListAsync();
        ViewBag.IdRom = IdRom;
        ViewBag.IdColor = IdColor;
        ViewBag.NewProducts = newProducts;
        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        ViewData["MenuName"] = new SelectList(_context.Menus, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel viewModel)
    {
        if (!ModelState.IsValid)
            // TODO: ADD error for model state
            return RedirectToAction("Create");

        var product = new Product
        {
            Name = viewModel.Name,
            Number = viewModel.Number,
            Price = viewModel.Price,
            OldPrice = viewModel.OldPrice,
            MenuId = viewModel.MenuId,
        };

        product.Image = $"{product.Id}_{viewModel.ImageFile.FileName}";
        string fileSavePath = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image);

        _context.Add(product);

        using var stream = new FileStream(fileSavePath, FileMode.Create);

        await Task.WhenAll(_context.SaveChangesAsync(), viewModel.ImageFile.CopyToAsync(stream));

        return RedirectToAction(nameof(Index));
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _context.Products
                                    .Include(p => p.Menu)
                                    .Include(p => p.DetailRoms)
                                    .ThenInclude(dr => dr.IdRomNavigation)
                                    .Include(x => x.DetailColors)
                                    .ThenInclude(xx => xx.IdColorNavigation)
                                    .Include(img => img.Images)
                                    .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        ViewData["MenuName"] = new SelectList(_context.Menus, "Id", "Name", product.MenuId);

        //return View(product);
        var editProductViewModel = new EditProductViewModel
        {
            ProductId = product.Id,
            Name = product.Name,
            Number = product.Number,
            Price = product.Price,
            OldPrice = product.OldPrice,
            MenuId = product.MenuId,
            OldImage = $"/Image/Product/{product.Image!}",
            Images = product.Images.Select(image => new WebBanHang.Models.Image
            {
                Id = image.Id,
                Img = image.Img,
            }).ToList()
        };
        return View(editProductViewModel); 
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProductViewModel viewModel)
    {
        var product = _context.Products
            .Where(p => p.Id == viewModel.ProductId).FirstOrDefault();

        if (product == null)
        {
            return NotFound();
        }

        product.Name = viewModel.Name;
        product.Number = viewModel.Number;
        product.Price = viewModel.Price;
        product.OldPrice = viewModel.OldPrice;
        product.MenuId = viewModel.MenuId;

        if (viewModel.ImageFile != null && product.Image != viewModel.ImageFile.FileName)
        {
            var oldImage = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image!);
            var newImage = $"{product.Id}_{viewModel.ImageFile.FileName}";
            
            product.Image = newImage;
            
            string fileSavePath = Path.Combine(_webHost.WebRootPath, "image", "Product", product.Image);
            using var stream = new FileStream(fileSavePath, FileMode.Create);

            _context.Update(product);
            
            await Task.WhenAll(_context.SaveChangesAsync(), viewModel.ImageFile.CopyToAsync(stream));

            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
        }
        else
        {
            _context.Update(product);
            
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
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
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.DetailColors)
            .Include(p=>p.DetailRoms)
            .FirstOrDefaultAsync(p=>p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Search(string keyword)
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
    [AllowAnonymous]
    public async Task<IActionResult> GetByMenu(string id)
    {
        ViewBag.MenuId = id;
        var products = _context.Products.Include(p => p.Menu)
            .Where(p => p.Menu!.Id.ToString() == id)
            .OrderByDescending(p => p.Created);
        ViewBag.MenuName = _context.Menus.FirstOrDefault(m => m.Id.ToString() == id)?.Name;
        return View(await products.ToListAsync());
    }
    public async Task<IActionResult> GetByMenuAdmin(string id)
    {
        ViewBag.MenuId = id;
        var products = _context.Products.Include(p => p.Menu)
            .Where(p => p.Menu!.Id.ToString() == id)
            .OrderByDescending(p => p.Created);
        ViewBag.MenuName = _context.Menus.FirstOrDefault(m => m.Id.ToString() == id)?.Name;
        return View(await products.ToListAsync());
    }

    public IActionResult SearchInMenu(string id, string keyword)
    {
        if (!string.IsNullOrEmpty(keyword))
        {
            IQueryable<Product> data = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(u => u.Menu!.Id.ToString() == id && u.Name.Contains(keyword));
            }

            return View(data);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddImages(Guid Id, IFormFile imageFile)
    {
        var img = new WebBanHang.Models.Image
        {
            Id = Guid.NewGuid(),
            IdProduct = Id
        };
        string uniqueFileName = $"{img.Id}_{Path.GetExtension(imageFile.FileName)}";
        img.Img = uniqueFileName;
        string fileSavePath = Path.Combine(_webHost.WebRootPath, "image", "Product", uniqueFileName);
        _context.Images.Add(img);
        using (var stream = new FileStream(fileSavePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> DeleteImage(Guid Id)
    {
        var image = _context.Images.Where(x => x.Id == Id).FirstOrDefault();

        if (image == null)
        {
            return NotFound();
        }
        string filePath = Path.Combine(_webHost.WebRootPath, "image", "Product", image.Img);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.Images.Remove(image);

        await _context.SaveChangesAsync();
        return RedirectToAction("Edit", new { id = image.IdProduct });

    }


}
