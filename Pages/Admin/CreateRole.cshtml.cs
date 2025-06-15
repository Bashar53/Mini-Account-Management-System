using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mini_Account_Management_System.Service;

namespace Mini_Account_Management_System.Pages.Admin;

[Authorize] // only basic authorization; detailed permission is dynamic
public class CreateRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PermissionService _permissionService;

    public CreateRoleModel(RoleManager<IdentityRole> roleManager,
                           UserManager<IdentityUser> userManager,
                           PermissionService permissionService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _permissionService = permissionService;
    }

    [BindProperty]
    public string RoleName { get; set; }

    [BindProperty]
    public List<string> SelectedUserIds { get; set; } = new();

    public List<IdentityUser> AllUsers { get; set; } = new();

    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var currentUrl = HttpContext.Request.Path.Value;
        bool canCreate = await _permissionService.HasPermissionByUrlAsync(currentUrl, "create");

        if (!canCreate)
            return Forbid();

        AllUsers = _userManager.Users.ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUrl = HttpContext.Request.Path.Value;
        bool canCreate = await _permissionService.HasPermissionByUrlAsync(currentUrl, "create");

        if (!canCreate)
            return Forbid();

        if (string.IsNullOrWhiteSpace(RoleName))
        {
            StatusMessage = "Role name is required.";
            AllUsers = _userManager.Users.ToList();
            return Page();
        }

        var exists = await _roleManager.RoleExistsAsync(RoleName);
        if (exists)
        {
            StatusMessage = $"The role '{RoleName}' already exists.";
            AllUsers = _userManager.Users.ToList();
            return Page();
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(RoleName));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to create role.");
            AllUsers = _userManager.Users.ToList();
            return Page();
        }

        foreach (var userId in SelectedUserIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, RoleName);
            }
        }

        TempData["StatusMessage"] = $"Role '{RoleName}' created successfully.";
        return RedirectToPage("/Admin/RoleList");
    }
}
