using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;

namespace Mini_Account_Management_System.Service
{
    public class PermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public PermissionService(ApplicationDbContext context,
                         IHttpContextAccessor httpContextAccessor,
                         UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<bool> HasPermissionAsync(int appResourceId, string action)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any()) return false;

            var role = roles.First(); // Support only one role for now

            var resource = await _context.AppResources.FirstOrDefaultAsync(r => r.AppResourceId == appResourceId);
            if (resource == null) return false;

            var perm = await _context.AppPermissions
                .FirstOrDefaultAsync(p => p.RoleId == _context.Roles.First(r => r.Name == role).Id &&
                                          p.AppResourceId == resource.AppResourceId);

            if (perm == null) return false;

            return action.ToLower() switch
            {
                "create" => perm.vCreate,
                "read" => perm.vRead,
                "update" => perm.vUpdate,
                "delete" => perm.vDelete,
                _ => false
            };
        }

    }
}
