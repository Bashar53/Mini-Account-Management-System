﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.Service
{
    public class PermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public PermissionService(
            ApplicationDbContext context,
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

            var roleName = roles.First(); // Single role support for now

            var roleId = await _context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(roleId)) return false;

            var perm = await _context.AppPermissions
                .FirstOrDefaultAsync(p => p.RoleId == roleId && p.AppResourceId == appResourceId);

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

        public async Task<bool> HasPermissionByUrlAsync(string fullPath, string action)
        {
            var normalizedUrl = NormalizePath(fullPath);

            var resource = await _context.AppResources
                .FirstOrDefaultAsync(r => r.Url == normalizedUrl);

            if (resource == null) return false;

            return await HasPermissionAsync(resource.AppResourceId, action);
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            // e.g., "/Admin/EditRole/123" → "/Admin/EditRole"
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return "/" + parts[0] + "/" + parts[1];
            }

            return "/" + string.Join("/", parts); // fallback
        }
        public async Task<List<AppResource>> GetMenuTreeAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return new();

            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault();
            if (string.IsNullOrEmpty(roleName)) return new();

            var roleId = await _context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var permittedResourceIds = await _context.AppPermissions
                .Where(p => p.RoleId == roleId && p.vRead)
                .Select(p => p.AppResourceId)
                .ToListAsync();

            var allResources = await _context.AppResources
                .Where(r => permittedResourceIds.Contains(r.AppResourceId))
                .OrderBy(r => r.MenuOrder) 
                .ToListAsync();

            // Build tree
            foreach (var res in allResources)
            {
                res.Children = new List<AppResource>();
            }

            var lookup = allResources.ToDictionary(r => r.AppResourceId);

            List<AppResource> rootNodes = new();

            foreach (var res in allResources)
            {
                if (res.ParentId != null && lookup.ContainsKey(res.ParentId))
                {
                    lookup[res.ParentId].Children.Add(res);
                }
                else
                {
                    rootNodes.Add(res);
                }
            }

            return rootNodes;
        }


    }

}
