using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roleManager.Roles.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Name
        }).ToListAsync();

        return View(new CreateUserViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel createUser)
    {
        if (!ModelState.IsValid) return RedirectToAction("Create");

        var user = new AppUser
        {
            FirstName = createUser.FirstName,
            LastName = createUser.LastName,
            UserName = createUser.UserName,
        };

        var result = await _userManager.CreateAsync(user, createUser.Password);
        if (!result.Succeeded)
            return RedirectToAction("Create");

        result = await _userManager.AddToRoleAsync(user, createUser.Role);

        if (!result.Succeeded)
            return RedirectToAction("Create");

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var userDetails = new EditUserViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = userRoles.ToList()
        };

        ViewBag.Roles = await _roleManager.Roles.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Name
        }).ToListAsync();

        return View(userDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Create");

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;

        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToRemove = currentRoles.Except(model.Roles).ToList();
        if (rolesToRemove.Count > 0)
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        var rolesToAdd = model.Roles.Except(currentRoles).ToList();
        if (rolesToAdd.Count > 0)
        {
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        return RedirectToAction("Index");
    }

    // public async Task<IActionResult> Details(string id)
    // {

    //     var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

    //     if (user == null)
    //     {
    //         return NotFound();
    //     }

    //     var roles = await _userManager.GetRolesAsync(user);
    //     var userWithRole = (new UserWithRoleVM
    //     {
    //         UserId = user.Id,
    //         FirstName = user.FirstName,
    //         LastName = user.LastName,
    //         Email = user.Email,
    //         Roles = roles.ToList()
    //     });

    //     return View(userWithRole);
    // }

    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
            return RedirectToAction("Index");

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("Error");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View(new LoginAdminViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginAdminViewModel viewModel)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, false, false);

        if (!signInResult.Succeeded)
        {
            // TODO: Add Errors here
            return RedirectToAction("Login");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // [HttpPost]
    // public async Task<IActionResult> Search(string keyword)
    // {
    //     if (!string.IsNullOrEmpty(keyword))
    //     {
    //         IQueryable<AppUser> data = _userManager.Users.AsQueryable();
    //         if (!string.IsNullOrEmpty(keyword))
    //         {
    //             data = data.Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword));
    //         }

    //         var usersWithRoles = new List<UserWithRoleVM>();

    //         foreach (var user in data)
    //         {
    //             var roles = await _userManager.GetRolesAsync(user);

    //             var userWithRole = new UserWithRoleVM
    //             {
    //                 UserId = user.Id,
    //                 FirstName = user.FirstName,
    //                 LastName = user.LastName,
    //                 Email = user.Email,
    //                 Roles = roles.ToList()
    //             };

    //             usersWithRoles.Add(userWithRole);
    //         }
    //         return View(usersWithRoles);
    //     }
    //     else
    //     {
    //         return RedirectToAction("Index");
    //     }
    // }
}
