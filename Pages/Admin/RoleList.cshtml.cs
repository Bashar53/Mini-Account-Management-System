using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ListRolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public List<IdentityRole> Roles { get; set; }

    public ListRolesModel(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public void OnGet()
    {
        Roles = _roleManager.Roles.ToList();
    }
}
