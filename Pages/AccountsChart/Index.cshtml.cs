using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Service;
using System;

namespace Mini_Account_Management_System.Pages.AccountsChart
{
    // Pages/Accounts/Index.cshtml.cs
    public class IndexModel : PageModel
    {
        private readonly AccountsChartService _ctx;
        private readonly PermissionService _permissionService;

        public List<Models.AccountsChart> RootNodes { get; private set; } = new();
        public IndexModel(AccountsChartService ctx, PermissionService permissionService)
            => (_ctx, _permissionService) = (ctx, permissionService);
        public async Task<IActionResult> OnGetAsync()
        {
            var currentUrl = HttpContext.Request.Path.Value;
            bool canRead = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!canRead)
                return Forbid();

            var flatList = await _ctx.GetAccountsChartAsync("SELECT");

            // Initialize Children list for all accounts to avoid null refs
            foreach (var acc in flatList)
            {
                acc.Children = new List<Models.AccountsChart>();
            }
            // Build a lookup dictionary by AccountId
            var lookup = flatList.ToDictionary(a => a.AccountId);

            // Build tree structure
            foreach (var acc in flatList)
            {
                if (acc.ParentId.HasValue && lookup.TryGetValue(acc.ParentId.Value, out var parent))
                {
                    parent.Children.Add(acc);
                }
            }

            // Root nodes = accounts with no parent
            RootNodes = flatList.Where(a => a.ParentId == null).ToList();
            return Page();
    }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var currentUrl = HttpContext.Request.Path.Value;
            bool canDelete = await _permissionService.HasPermissionByUrlAsync(currentUrl, "delete");
            if (!canDelete)
                return Forbid();
            await _ctx.DeleteAccountAsync(id); 
            return RedirectToPage();
        }
    }

}
