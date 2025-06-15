using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Service;
using Microsoft.AspNetCore.Authorization;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize]
public class AssignPermissionsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PermissionService _permissionService;

    public AssignPermissionsModel(ApplicationDbContext context,
                                   RoleManager<IdentityRole> roleManager,
                                   UserManager<IdentityUser> userManager,
                                   PermissionService permissionService)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _permissionService = permissionService;
    }

    [BindProperty(SupportsGet = true)]
    public string SelectedRoleId { get; set; }

    [BindProperty]
    public List<PermissionWrapper> Permissions { get; set; } = new();

    public List<SelectListItem> Roles { get; set; }
    public List<AppResource> Resources { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var url = HttpContext.Request.Path.Value;
        bool canRead = await _permissionService.HasPermissionByUrlAsync(url, "read");
        if (!canRead)
            return Forbid();

        Roles = await _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Id, Text = r.Name })
                .ToListAsync();

        Resources = await _context.AppResources
                .Where(r => r.MenuIsVisible == true && r.IsActive == true)
                .OrderBy(r => r.MenuOrder)
                .ToListAsync();

        var existingPermissions = new List<AppPermission>();
        if (!string.IsNullOrEmpty(SelectedRoleId))
        {
            existingPermissions = await _context.AppPermissions
                .Where(p => p.RoleId == SelectedRoleId)
                .ToListAsync();
        }

        Permissions = Resources.Select(resource =>
        {
            var existing = existingPermissions.FirstOrDefault(p => p.AppResourceId == resource.AppResourceId);

            return new PermissionWrapper
            {
                AppResourceId = resource.AppResourceId,
                Permissions = existing ?? new AppPermission
                {
                    RoleId = SelectedRoleId,
                    AppResourceId = resource.AppResourceId
                }
            };
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var url = HttpContext.Request.Path.Value;
        bool canUpdate = await _permissionService.HasPermissionByUrlAsync(url, "update");
        if (!canUpdate)
            return Forbid();

        foreach (var entry in Permissions)
        {
            var resId = entry.AppResourceId;
            var perm = entry.Permissions;

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC sp_AssignPermissions 
                    @RoleId = {SelectedRoleId},
                    @AppResourceId = {resId},
                    @vCreate = {perm.vCreate},
                    @vRead = {perm.vRead},
                    @vUpdate = {perm.vUpdate},
                    @vDelete = {perm.vDelete}
            ");
        }

        TempData["StatusMessage"] = "Permissions assigned successfully.";
        return RedirectToPage("/Admin/AssignPermissions", new { SelectedRoleId });
    }
}
