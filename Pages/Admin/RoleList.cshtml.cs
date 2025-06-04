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
        var userId = _userManager.GetUserId(User);

        // Match the current page with the AppResources entry (by Url)
        var resource = await _context.AppResources.FirstOrDefaultAsync(r => r.Url == "/Admin/RoleList");

        if (resource == null)
        {
            return Forbid(); // No resource mapping → deny access
        }

        // Check permissions for the current user
        var permission = await _context.AppPermissions
            .FirstOrDefaultAsync(p => p.UserId == userId && p.AppResourceId == resource.AppResourceId);

        if (permission?.vRead == true)
        {
            CanRead = true;
            Roles = _roleManager.Roles.ToList();
            return Page();
        }

        return Forbid();
    }
}
