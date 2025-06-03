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

    public CreateRoleModel(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [BindProperty]
    public string RoleName { get; set; }

    public string Message { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(RoleName))
        {
            Message = "Role name is required.";
            return Page();
        }

        var exists = await _roleManager.RoleExistsAsync(RoleName);
        if (exists)
        {
            Message = $"The role '{RoleName}' already exists.";
            return Page();
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(RoleName));
        Message = result.Succeeded ? $"Role '{RoleName}' created successfully." : "Failed to create role.";

        return Page();
    }
}
