using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Service;
using System;

namespace Mini_Account_Management_System.Pages.AccountsChart
{
    public class UpsertModel : PageModel
    {
        private readonly AccountsChartService _service;
        [BindProperty] public Models.AccountsChart Input { get; set; } = new();

        public SelectList ParentOptions { get; private set; } = default!;

        public UpsertModel(AccountsChartService service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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

                var action = Input.AccountId == 0 ? "CREATE" : "UPDATE";

                await _service.GetAccountsChartAsync(
                    action,
                    Input.AccountId,
                    Input.AccountName,
                    Input.ParentId);

                return RedirectToPage("Index");
            }
        }
    }


