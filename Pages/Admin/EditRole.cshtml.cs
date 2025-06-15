using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditRoleModel(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string RoleId { get; set; }  

        [BindProperty]
        public string RoleName { get; set; }

        [BindProperty]
        public List<string> SelectedUserIds { get; set; } = new();

        [BindProperty]
        public List<PermissionWrapper> Permissions { get; set; } = new();

        public List<IdentityUser> AllUsers { get; set; } = new();
        public List<AppResource> Resources { get; set; } = new();
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(RoleId))
                return NotFound();

            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
                return NotFound();

            RoleName = role.Name;

            // Load users and permissions
            AllUsers = await _userManager.Users.ToListAsync();
            SelectedUserIds = await _userManager.GetUsersInRoleAsync(role.Name).ContinueWith(t => t.Result.Select(u => u.Id).ToList());

            Resources = await _context.AppResources
                            .Where(r => r.IsActive && r.MenuIsVisible)
                            .OrderBy(r => r.MenuOrder)
                            .ToListAsync();

            var existingPermissions = await _context.AppPermissions
                .Where(p => p.RoleId == RoleId)
                .ToListAsync();

            Permissions = Resources.Select(resource =>
            {
                var existing = existingPermissions.FirstOrDefault(p => p.AppResourceId == resource.AppResourceId);
                return new PermissionWrapper
                {
                    AppResourceId = resource.AppResourceId,
                    Permissions = existing ?? new AppPermission
                    {
                        RoleId = RoleId,
                        AppResourceId = resource.AppResourceId
                    }
                };
            }).ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
                return NotFound();

            // Update Role Name if changed
            if (!string.IsNullOrWhiteSpace(RoleName) && role.Name != RoleName)
            {
                role.Name = RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update role name.");
                    return Page();
                }
            }

            // Update user assignments
            var allUsers = await _userManager.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                var inRole = await _userManager.IsInRoleAsync(user, role.Name);
                var shouldBeInRole = SelectedUserIds.Contains(user.Id);

                if (inRole && !shouldBeInRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else if (!inRole && shouldBeInRole)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            // Update permissions
            foreach (var entry in Permissions)
            {
                var perm = entry.Permissions;
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_AssignPermissions 
                        @RoleId = {RoleId},
                        @AppResourceId = {entry.AppResourceId},
                        @vCreate = {perm.vCreate},
                        @vRead = {perm.vRead},
                        @vUpdate = {perm.vUpdate},
                        @vDelete = {perm.vDelete}
                ");
            }

            TempData["StatusMessage"] = $"Role '{role.Name}' updated successfully.";
            return RedirectToPage("/Admin/RoleList");
        }
    }
}
