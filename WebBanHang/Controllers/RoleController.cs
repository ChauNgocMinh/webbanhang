using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Identity.Data;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        // GET: RoleController
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var rolesvm = new List<RoleVM>();
            foreach(var role in roles)
            {
                var rolevm = new RoleVM
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };
                rolesvm.Add(rolevm);
            }
            
            return View(rolesvm);
        }

        // GET: RoleController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var rolevm = new RoleVM
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
            return View(rolevm);
        }

        // GET: RoleController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleVM model)
        {
            var role = new IdentityRole()
            {
                Name = model.RoleName
            };
            await _roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }

        // GET: RoleController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var rolevm = new RoleVM
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
            return View(rolevm);
        }

        // POST: RoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleVM model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.RoleId);
            if(role != null) 
            { 
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach(var error in result.Errors) 
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Edit", role);
                }
            }
            else
            {
                return NotFound();
            }
        }

        // POST: RoleController/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Edit", role);
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            IQueryable<IdentityRole> data = _roleManager.Roles.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(u => u.Name.Contains(keyword));
            }

            var rolesvm = new List<RoleVM>();

            foreach (var role in data)
            {
                var rolevm = new RoleVM
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };

                rolesvm.Add(rolevm);
            }
            return View(rolesvm);
        }
    }
}
