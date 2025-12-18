using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public enum AccountType
    {
        Normal = 0,   // All other accounts (cash, AP, AR, expenses, income, etc.)
        INTY = 1,     // Intangible Asset
        CIP = 2,      // Construction in Progress
        FA = 3,       // Fixed Asset
        Accu = 4      // Accumulated / Contra Asset
    }
    public class ChartofAccount
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [DisplayName("Account Name")]
        public string AccountName { get; set; }
        public bool HasSubsidiary { get; set; }
        public bool Debit { get; set; }
        [Required]
        public AccountType AccountType { get; set; }

        // Depreciation FK
        [ValidateNever]
        public int? DepreciationExpenseAccountId { get; set; }
        [ValidateNever]
        public ChartofAccount DepreciationExpenseAccount { get; set; }
        [ValidateNever]
        public int? AccumulatedDepreciationAccountId { get; set; }
        [ValidateNever]
        public ChartofAccount AccumulatedDepreciationAccount { get; set; }

        // NEW: reverse navigation
        [ValidateNever]
        public ICollection<ChartofAccount> FAUsingThisAsDepreciation { get; set; }
        [ValidateNever]
        public ICollection<ChartofAccount> FAUsingThisAsAccumulated { get; set; }

        [ValidateNever]
        public virtual ICollection<SubsidiaryAccount> Subsidiaries { get; set; }
        [ValidateNever]
        public virtual ICollection<JevEntry> JevEntries { get; set; }
    }

}
