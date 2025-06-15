using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Service;
using System.Data;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize]
public class ListRolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PermissionService _permissionService;



    public List<IdentityRole> Roles { get; set; }
    public bool CanRead { get; set; } = false;

    public ListRolesModel(RoleManager<IdentityRole> roleManager,
                           PermissionService permissionService)
    {
        _roleManager = roleManager;
        _permissionService = permissionService;
    }
        public async Task<IActionResult> OnGet()
        {
            var currentUrl = HttpContext.Request.Path.Value;

            var hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!hasPermission)
                return Forbid();

            CanRead = true;
            Roles = _roleManager.Roles.ToList();
            return Page();
        }
}
