using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Mini_Account_Management_System.DbConnection
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
