using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mini_Account_Management_System.Service;

namespace Mini_Account_Management_System.Pages.Admin
{
    [Authorize]
    public class EditRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PermissionService _permissionService;

        public EditRoleModel(RoleManager<IdentityRole> roleManager,
                             UserManager<IdentityUser> userManager,
                             PermissionService permissionService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _permissionService = permissionService;
        }

        [BindProperty]
        public string RoleId { get; set; }

        [BindProperty]
        public string RoleName { get; set; }

        public List<UserCheckbox> Users { get; set; } = new();

        [BindProperty]
        public List<string> SelectedUserIds { get; set; } = new();

        public class UserCheckbox
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public bool IsSelected { get; set; }
        }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Permission check (Update implies Read)
            var hasPermission = await _permissionService.HasPermissionByUrlAsync(HttpContext.Request.Path, "update");
            if (!hasPermission)
                return Forbid();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound("Role not found.");

            RoleId = role.Id;
            RoleName = role.Name;

            foreach (var user in _userManager.Users.ToList())
            {
                Users.Add(new UserCheckbox
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, RoleName)
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hasPermission = await _permissionService.HasPermissionByUrlAsync(HttpContext.Request.Path, "update");
            if (!hasPermission)
                return Forbid();

            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
                return NotFound("Role not found.");

            // Update Role Name
            if (role.Name != RoleName)
            {
                role.Name = RoleName;
                var result = await _roleManager.UpdateAsync(role);
                StatusMessage = result.Succeeded ? "Role updated successfully." : "Failed to update role.";
            }

            // Update Users in Role
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                bool isInRole = await _userManager.IsInRoleAsync(user, RoleName);
                bool shouldBeInRole = SelectedUserIds.Contains(user.Id);

                if (shouldBeInRole && !isInRole)
                    await _userManager.AddToRoleAsync(user, RoleName);

                if (!shouldBeInRole && isInRole)
                    await _userManager.RemoveFromRoleAsync(user, RoleName);
            }

            TempData["StatusMessage"] = "Role updated and users assigned successfully.";
            return RedirectToPage("/Admin/RoleList");
        }
    }
}
