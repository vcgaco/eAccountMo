using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace eAccount.Models
{
    public class JevEntry
    {
        public int Id { get; set; }

        public int JevId { get; set; }                 // FK → JEV Header

        public int AccountId { get; set; }             // FK → COA

        public int? SubsidiaryId { get; set; }         // FK → SubsidiaryLedger, optional
        public int? FixedAssetId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; }

        // Navigation
        [ValidateNever]
        public virtual Jev Jev { get; set; }
        [ValidateNever]
        public virtual ChartofAccount Account { get; set; }
        [ValidateNever]
        public virtual SubsidiaryAccount Subsidiary { get; set; }
        [ValidateNever]
        public FixedAsset FixedAsset { get; set; }
    }
}
