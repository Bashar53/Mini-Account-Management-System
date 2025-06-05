using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ListRolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;



    public List<IdentityRole> Roles { get; set; }
    public bool CanRead { get; set; } = false;

    public ListRolesModel(RoleManager<IdentityRole> roleManager,
                      ApplicationDbContext context,
                      UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGet()
    {
        var role = await _roleManager.FindByNameAsync("Admin");
        var roleId = await _roleManager.GetRoleIdAsync(role);
        // Match the current page with the AppResources entry (by Url)
        var resource = await _context.AppResources.FirstOrDefaultAsync(r => r.Url == "/Admin/RoleList");

        if (resource == null)
        {
            return Forbid(); // No resource mapping → deny access
        }
        var permission = await _context.AppPermissions
            .FirstOrDefaultAsync(p => p.RoleId == roleId && p.AppResourceId == resource.AppResourceId);

        if (permission?.vRead == true)
        {
            CanRead = true;
            Roles = _roleManager.Roles.ToList();
            return Page();
        }

        return Forbid();
    }
}
