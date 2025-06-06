using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.DbConnection
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<AppResource> AppResources { get; set; }
        public DbSet<AppPermission> AppPermissions { get; set; }    
        public DbSet<AccountsChart> AccountsChart { get; set; }    
    }
}
