using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Identity.Data;
using WebBanHang.Areas.Identity.Pages.Account;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<WebBanHangUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<WebBanHangUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //danh sách user
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRoleVM>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRoleVM
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(usersWithRoles);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userWithRole = (new UserWithRoleVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles.ToList()
            });
            ViewBag.RoleList = _roleManager.Roles.ToList();

            return View(userWithRole);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserWithRoleVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToAdd = model.Roles.Except(currentRoles);
                var rolesToRemove = currentRoles.Except(model.Roles);

                await _userManager.AddToRolesAsync(user, rolesToAdd);
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i,
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new WebBanHangUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Thêm người dùng vào một vai trò nếu cần
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                });
                return View("Create", model);
            }
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userWithRole = (new UserWithRoleVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles.ToList()
            });

            return View(userWithRole);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Customer");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword)) {
                IQueryable<WebBanHangUser> data = _userManager.Users.AsQueryable();
                if (!string.IsNullOrEmpty(keyword))
                {
                    data = data.Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword));
                }

                var usersWithRoles = new List<UserWithRoleVM>();

                foreach (var user in data)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var userWithRole = new UserWithRoleVM
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Roles = roles.ToList()
                    };

                    usersWithRoles.Add(userWithRole);
                }
                return View(usersWithRoles);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
