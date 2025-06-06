using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.Service
{
    public class AccountsChartService
    {
        private readonly ApplicationDbContext _db;

        public AccountsChartService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<AccountsChart>> GetAccountsChartAsync(
    string action, int id = 0, string name = null, int? parentId = null)
        {
            return await _db.AccountsChart
                .FromSqlInterpolated($"EXEC dbo.sp_ManageChartOfAccounts {action}, {id}, {name}, {parentId}")
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task DeleteAccountAsync(int id)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.sp_ManageChartOfAccounts 'DELETE', {id}, {null}, {null}");
        }
    }
}
