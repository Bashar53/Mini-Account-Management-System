using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Account_Management_System.Models
{
        public class AccountsChart  
        {
            [Key]
            public int AccountId { get; set; }
            public string AccountName { get; set; } = default!;
            public int? ParentId { get; set; }
            public AccountsChart? Parent { get; set; }

            [NotMapped]
            public List<AccountsChart> Children { get; set; } = new();
    }
    
}
