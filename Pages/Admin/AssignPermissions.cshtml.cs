using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using System;

namespace Mini_Account_Management_System.Pages.Admin
{
    public class AssignPermissionsModel : PageModel
    {
            private readonly ApplicationDbContext _context;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<IdentityUser> _userManager;
        public AssignPermissionsModel(ApplicationDbContext context, RoleManager<IdentityRole> roleManager , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [BindProperty(SupportsGet = true)]
        public string SelectedRoleId { get; set; }

        [BindProperty]
        public List<PermissionWrapper> Permissions { get; set; } = new();
        public List<SelectListItem> Roles { get; set; }
        public List<AppResource> Resources { get; set; }
        public async Task OnGetAsync()
         {
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
                // Check if permission already exists
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
        }
            public async Task<IActionResult> OnPostAsync()
            {
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
                return RedirectToPage("/Admin/AssignPermissions");
            }
        }
    }

