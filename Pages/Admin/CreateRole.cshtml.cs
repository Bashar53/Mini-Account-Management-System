using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mini_Account_Management_System.Pages.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class CreateRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateRoleModel(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [BindProperty]
    public string RoleName { get; set; }
    [BindProperty]
    public List<string> SelectedUserIds { get; set; } = new();

    public List<IdentityUser> AllUsers { get; set; } = new();

    public string StatusMessage { get; set; }

    public void OnGet() {
        AllUsers = _userManager.Users.ToList();
    }
    public async Task<IActionResult> OnPostAsync()
    {
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
        StatusMessage = result.Succeeded ? $"Role '{RoleName}' created successfully." : "Failed to create role.";
        TempData["StatusMessage"] = StatusMessage;
        return RedirectToPage("/Admin/RoleList");
    }
}
