using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Service;
using System;
using System.Data;

namespace Mini_Account_Management_System.Pages.Voucher
{
    public class VoucherEntryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permissionService;
        public VoucherEntryModel(ApplicationDbContext context, PermissionService permissionService) 
        {
            _context = context;
            _permissionService = permissionService;
        }
        [BindProperty] public string VoucherType { get; set; }
        [BindProperty] public DateTime? Date { get; set; }
        [BindProperty] public string ReferenceNo { get; set; }
        [BindProperty] public List<VoucherEntry> Entries { get; set; } = new();
        public List<SelectListItem> AccountList { get; set; } = new();
        public async Task<IActionResult> OnGet()
        {
            var currentUrl = HttpContext.Request.Path.Value;

            var hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!hasPermission)
                return Forbid();
            hasPermission = true;
            LoadAccounts();
            Entries.Add(new VoucherEntry());
            return Page();
        }
        private void LoadAccounts()
        {
            var conn = _context.Database.GetDbConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AccountId, AccountName FROM AccountsChart";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                AccountList.Add(new SelectListItem
                {
                    Value = reader["AccountId"].ToString(),
                    Text = reader["AccountName"].ToString()
                });
            }

            conn.Close();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUrl = HttpContext.Request.Path.Value;
            var hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, "create");
            if (!hasPermission)
                return Forbid();
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "sp_SaveVoucher";
            command.CommandType = CommandType.StoredProcedure;

            // Scalar parameters (with types)
            command.Parameters.Add(new SqlParameter("@VoucherType", SqlDbType.NVarChar) { Value = VoucherType ?? "" });
            command.Parameters.Add(new SqlParameter("@Date", SqlDbType.Date) { Value = Date });
            command.Parameters.Add(new SqlParameter("@ReferenceNo", SqlDbType.NVarChar) { Value = ReferenceNo ?? "" });

            // Table-valued parameter
            var table = new DataTable();
            table.Columns.Add("AccountId", typeof(int));
            table.Columns.Add("Debit", typeof(decimal));
            table.Columns.Add("Credit", typeof(decimal));

            foreach (var entry in Entries)
            {
                if (entry.AccountId > 0)
                {
                    table.Rows.Add(entry.AccountId, entry.Debit ?? 0, entry.Credit ?? 0);
                }
            }

            var tvpParam = new SqlParameter("@Entries", table)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "VoucherEntryType"
            };

            command.Parameters.Add(tvpParam);
            try
            {

                await command.ExecuteNonQueryAsync();
            }
            catch
            {

            }

            TempData["SuccessMessage"] = "Voucher saved successfully.";
            return RedirectToPage("/Voucher/ExportVoucher");
        }

    }
}
