using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public class FinancialReportClassification
    {
        [Key]
        public int id { get; set; }
        public string ReportType { get; set; }
        public string Section { get; set; }
        public string SubSection { get; set; }
        public string Category { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int DisplayOrder { get; set; }
    }
}
