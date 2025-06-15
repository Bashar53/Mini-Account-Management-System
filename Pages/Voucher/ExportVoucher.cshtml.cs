using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Service;
using OfficeOpenXml;

namespace Mini_Account_Management_System.Pages.Voucher
{
    public class ExportVoucherModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permissionService;

        public ExportVoucherModel(ApplicationDbContext context, PermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? VoucherType { get; set; }

        public List<VoucherReportDto> VoucherList { get; set; } = new();

        // This runs when page loads or user presses Search button
        public async Task<IActionResult> OnGetAsync()
        {
            var currentUrl = HttpContext.Request.Path.Value;
            var hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!hasPermission)
                return Forbid();
            await LoadVouchersAsync();
            return Page();
        }
        // Called when user clicks Export Excel
        public async Task<IActionResult> OnGetExportAsync()
        {
            var currentUrl = HttpContext.Request.Path.Value;
            var hasPermission = await _permissionService.HasPermissionByUrlAsync(currentUrl, "read");
            if (!hasPermission)
                return Forbid();
            await LoadVouchersAsync(); // Get the same filtered data

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Vouchers");

            // Header row
            sheet.Cells["A1"].Value = "Voucher Type";
            sheet.Cells["B1"].Value = "Date";
            sheet.Cells["C1"].Value = "Reference No";
            sheet.Cells["D1"].Value = "Account";
            sheet.Cells["E1"].Value = "Debit";
            sheet.Cells["F1"].Value = "Credit";
            sheet.Cells["A1:F1"].Style.Font.Bold = true;

            decimal totalDebit = 0;
            decimal totalCredit = 0;
            int row = 2;

            foreach (var item in VoucherList)
            {
                sheet.Cells[row, 1].Value = item.VoucherType;
                sheet.Cells[row, 2].Value = item.Date.ToShortDateString();
                sheet.Cells[row, 3].Value = item.ReferenceNo;
                sheet.Cells[row, 4].Value = item.Account;
                sheet.Cells[row, 5].Value = item.Debit;
                sheet.Cells[row, 6].Value = item.Credit;

                totalDebit += item.Debit;
                totalCredit += item.Credit;
                row++;
            }

            sheet.Cells[row, 4].Value = "TOTAL:";
            sheet.Cells[row, 5].Value = totalDebit;
            sheet.Cells[row, 6].Value = totalCredit;
            sheet.Cells[row, 4, row, 6].Style.Font.Bold = true;

            sheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"VoucherReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task LoadVouchersAsync()
        {
            var fromParam = new SqlParameter("@FromDate", (object?)FromDate ?? DBNull.Value);
            var toParam = new SqlParameter("@ToDate", (object?)ToDate ?? DBNull.Value);
            var typeParam = new SqlParameter("@VoucherType", (object?)VoucherType ?? DBNull.Value);

            VoucherList = await _context.VoucherReportDto
                .FromSqlRaw("EXEC sp_GetVoucherReport @FromDate, @ToDate, @VoucherType", fromParam, toParam, typeParam)
                .ToListAsync();
        }
    }
}
