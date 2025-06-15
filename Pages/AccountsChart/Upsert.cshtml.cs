using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Service;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Account_Management_System.Pages.AccountsChart
{
    [Authorize]
    public class UpsertModel : PageModel
    {
        private readonly AccountsChartService _service;
        private readonly PermissionService _permissionService;

        [BindProperty]
        public Models.AccountsChart Input { get; set; } = new();

        public SelectList ParentOptions { get; private set; } = default!;

        public UpsertModel(AccountsChartService service, PermissionService permissionService)
        {
            _service = service;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check read permission
            var currentUrl = HttpContext.Request.Path.Value;
            bool canRead = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!canRead)
                return Forbid();

            var allAccounts = await _service.GetAccountsChartAsync("SELECT");

            ParentOptions = new SelectList(allAccounts,
                                           nameof(Models.AccountsChart.AccountId),
                                           nameof(Models.AccountsChart.AccountName));

            if (id is not null)
            {
                Input = allAccounts.First(a => a.AccountId == id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var currentUrl = HttpContext.Request.Path.Value;
            var action = Input.AccountId == 0 ? "create" : "update";

            // Check create or update permission based on action
            bool hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, action);
            if (!hasPermission)
                return Forbid();

            await _service.GetAccountsChartAsync(
                action.ToUpper(),  // "CREATE" or "UPDATE"
                Input.AccountId,
                Input.AccountName,
                Input.ParentId);

            return RedirectToPage("Index");
        }
    }
}
