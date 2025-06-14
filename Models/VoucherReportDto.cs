using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models
{
    public class VoucherReportDto
    {
        [Key]
        public string VoucherType { get; set; } = "";
        public DateTime Date { get; set; }
        public string ReferenceNo { get; set; } = "";
        public string Account { get; set; } = "";
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
