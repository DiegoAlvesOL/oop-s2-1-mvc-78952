using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.mvc.Controllers;

// Apenas utilizadores com role Admin podem aceder a este controller
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // RoleManager é o serviço do Identity que gerencia as roles
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    //lista todas as roles
    public IActionResult Roles()
    {
        var allRoles = _roleManager.Roles.ToList();
        return View(allRoles);
    }

    //Cria uma nova role
    [HttpPost]
    public async Task<IActionResult> CreateRole(string newRoleName)
    {
        if (!string.IsNullOrEmpty(newRoleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(newRoleName));
        }

        return RedirectToAction(nameof(Roles));
    }

    //Deleta uma role
    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        
        var roleToDelete = await _roleManager.FindByIdAsync(roleId);

        if (roleToDelete != null)
        {
            await _roleManager.DeleteAsync(roleToDelete);
        }

        return RedirectToAction(nameof(Roles));
    }
}