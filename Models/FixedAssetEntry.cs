using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public class FixedAssetEntry
    {
        [Key]
        public int Id { get; set; }

        public int SubsidiaryEntryId { get; set; }
        public int FixedAssetId { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string FixedAssetCode { get; set; }

        [ValidateNever]
        public SubsidiaryEntry SubsidiaryEntry { get; set; }

        [ValidateNever]
        public FixedAsset FixedAsset { get; set; }
    }
}
