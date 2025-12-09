using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models
{
    public class SubsidiaryAccount
    {
        [Key]
        public int Id { get; set; }

        public int AccountId { get; set; }
        public string SubsidiaryName { get; set; }
        public string SubsidiaryCode { get; set; }
        [ValidateNever]
        public virtual ChartofAccount Account { get; set; }
        [ValidateNever]
        public virtual ICollection<JevEntry> JevEntries { get; set; }
    }
}
