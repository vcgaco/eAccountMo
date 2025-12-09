using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public class ChartofAccount
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [DisplayName("Account Name")]
        public string AccountName { get; set; }
        [DisplayName("Enable Subsidiary")]
        public bool HasSubsidiary { get; set; }
        public bool Debit { get; set; }
        [ValidateNever]
        public virtual ICollection<SubsidiaryAccount> Subsidiaries { get; set; }
        [ValidateNever]
        public virtual ICollection<JevEntry> JevEntries { get; set; }
    }
}
