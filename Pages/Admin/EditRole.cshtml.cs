using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mini_Account_Management_System.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public EditRoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [BindProperty]
        public string RoleId { get; set; }

        [BindProperty]
        public string RoleName { get; set; }

        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            RoleId = role.Id;
            RoleName = role.Name;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            if (role.Name != RoleName)
            {
                role.Name = RoleName;
                var result = await _roleManager.UpdateAsync(role);
                StatusMessage = result.Succeeded ? "? Role updated successfully." : "? Failed to update role.";
            }
            else
            {
                StatusMessage = "No changes were made.";
            }

            return Page();
        }


        
    }
}
