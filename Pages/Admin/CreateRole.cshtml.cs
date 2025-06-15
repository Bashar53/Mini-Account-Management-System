using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Service;
using Mini_Account_Management_System.DbConnection;

namespace Mini_Account_Management_System.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public CreateRoleModel(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            PermissionService permissionService,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _permissionService = permissionService;
            _context = context;
        }

        [BindProperty]
        public string RoleName { get; set; }

        [BindProperty]
        public List<string> SelectedUserIds { get; set; } = new();

        [BindProperty]
        public List<PermissionWrapper> Permissions { get; set; } = new();

        public List<AppResource> Resources { get; set; } = new();

        public List<IdentityUser> AllUsers { get; set; } = new();

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            AllUsers = _userManager.Users.ToList();

            Resources = await _context.AppResources
                .Where(r => r.MenuIsVisible && r.IsActive)
                .OrderBy(r => r.MenuOrder)
                .ToListAsync();

            Permissions = Resources.Select(resource => new PermissionWrapper
            {
                AppResourceId = resource.AppResourceId,
                Permissions = new AppPermission()
            }).ToList();

            bool canCreate = await _permissionService.HasPermissionAsync(3, "create");
            if (!canCreate)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AllUsers = _userManager.Users.ToList();

            Resources = await _context.AppResources
                .Where(r => r.MenuIsVisible && r.IsActive)
                .OrderBy(r => r.MenuOrder)
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(RoleName))
            {
                StatusMessage = "Role name is required.";
                return Page();
            }

            var exists = await _roleManager.RoleExistsAsync(RoleName);
            if (exists)
            {
                StatusMessage = $"The role '{RoleName}' already exists.";
                return Page();
            }

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(RoleName));
            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to create role.");
                return Page();
            }

            var role = await _roleManager.FindByNameAsync(RoleName);

            foreach (var userId in SelectedUserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, RoleName);
                }
            }

            foreach (var entry in Permissions)
            {
                var perm = entry.Permissions;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_AssignPermissions 
                        @RoleId = {role.Id},
                        @AppResourceId = {entry.AppResourceId},
                        @vCreate = {perm.vCreate},
                        @vRead = {perm.vRead},
                        @vUpdate = {perm.vUpdate},
                        @vDelete = {perm.vDelete}
                ");
            }

            TempData["StatusMessage"] = $"Role '{RoleName}' created with permissions.";
            return RedirectToPage("/Admin/RoleList");
        }
    }
}
