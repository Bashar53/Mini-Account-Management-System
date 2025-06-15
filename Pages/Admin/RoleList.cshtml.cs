using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using System.Data;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize]
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
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge(); // Not logged in

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any())
            return Forbid();

        var currentUrl = HttpContext.Request.Path.Value.ToLower();
        var resource = await _context.AppResources
            .FirstOrDefaultAsync(r => r.Url.ToLower() == currentUrl);

        if (resource == null)
        {
            return Forbid(); // No resource mapping → deny access
        }
        bool hasReadPermission = false;
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var permission = await _context.AppPermissions
                .FirstOrDefaultAsync(p => p.RoleId == role.Id && p.AppResourceId == resource.AppResourceId);

            if (permission?.vRead == true)
            {
                hasReadPermission = true;
                break;
            }
        }
        if (!hasReadPermission)
            return Forbid();

        CanRead = true;
        Roles = _roleManager.Roles.ToList();
        return Page();

        
    }
}
