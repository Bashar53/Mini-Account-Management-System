using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models
{
    public class AppPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppPermissionId { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public int AppResourceId { get; set; }
        public bool vCreate { get; set; }
        public bool vRead { get; set; }
        public bool vUpdate { get; set; }
        public bool vDelete { get; set; }
    }
}
