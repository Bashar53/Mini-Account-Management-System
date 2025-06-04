using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models;

public class AppResource
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppResourceId { get; set; }
        public int ParentId { get; set; }
        public string ModelName { get; set; }
        public string DisplayName { get; set; }
        public int? MenuOrder { get; set; } = 0;
        public bool IsModel { get; set; }
        public string IconClass { get; set; } = "fa-solid fa-bars";
        public string Url { get; set; }
        public bool? MenuIsVisible { get; set; } = false;
        public bool? IsActive { get; set; } = false;
        public string Position { get; set; } = "Top";
        public bool? HasLeftMenu { get; set; } = false;
        public bool? HasSeparator { get; set; } = false;
        [NotMapped] public bool vCreate { get; set; }
        [NotMapped] public bool vRead { get; set; }
        [NotMapped] public bool vUpdate { get; set; }
        [NotMapped] public bool vDelete { get; set; }
        [NotMapped] public List<AppResource> Children { get; set; } = new();
}
