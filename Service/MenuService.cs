using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using System;

namespace Mini_Account_Management_System.Service;

public class MenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<AppResource> GetUserMenu(string userId)
    {
        var resources = _context.AppResources
            .FromSqlInterpolated($"EXEC GetUserResources @UserId = {userId}")
            .AsEnumerable()
            .ToList();

        return BuildTree(null, resources);
    }

    private List<AppResource> BuildTree(int? parentId, List<AppResource> flat)
    {
        return flat
            .Where(r => r.ParentId == (parentId ?? 0))
            .Select(r => {
                r.Children = BuildTree(r.AppResourceId, flat);
                return r;
            }).ToList();
    }
}
