using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public class FixedAsset
    {
        [Key]
        public int Id { get; set; }

        // ✅ FK → Subsidiary ONLY (NO JEV LINK)
        public int SubsidiaryAccountId { get; set; }

        public string ChildCode { get; set; }    // e.g. ICT-001
        public string ChildName { get; set; }    // e.g. Dell Inspiron 15

        public string SerialNumber { get; set; }
        public string Model { get; set; }

        public string PropertyNo { get; set; }

        public int? UsefulLife { get; set; }

        public DateTime? AcquisitionDate { get; set; }
        public DateTime? DepreciationEffectivity { get; set; }

        public decimal? Amount { get; set; }
        public decimal? ScrapValue { get; set; }
        public decimal? AnnualDepreciation { get; set; }
        public decimal? MonthlyDepreciation { get; set; }

        [ValidateNever]
        public virtual SubsidiaryAccount SubsidiaryAccount { get; set; }
        [ValidateNever]
        public ICollection<JevEntry> JevEntries { get; set; }
    }
}
