using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mini_Account_Management_System.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public DeleteRoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [BindProperty]
        public IdentityRole Role { get; set; }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Role = await _roleManager.FindByIdAsync(id);
            if (Role == null)
            {
                return NotFound("Role not found.");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var role = await _roleManager.FindByIdAsync(Role.Id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = $"Role '{role.Name}' deleted.";
                return RedirectToPage("/Admin/RoleList");
            }
            StatusMessage = $"Error deleting role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            return Page();
        }
    }
}
