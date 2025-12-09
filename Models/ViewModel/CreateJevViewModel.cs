using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace eAccount.Models.ViewModel
{
    public class CreateJevViewModel
    {
        [Required]
        [Display(Name = "JEV Number")]
        public string JevNo { get; set; }

        [Required]
        [Display(Name = "Fund")]
        public int? FundId { get; set; }  // just an int

        [Display(Name = "Special Account")]
        public int? SpecialAccountId { get; set; } // nullable

        [Required]
        [Display(Name = "Amount")]
        public decimal JevAmount { get; set; }

        [Required]
        public string Particular { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Accounting Period")]
        public DateTime AccountingPeriod { get; set; } = DateTime.Now;
        public DateTime EncodedDate { get; set; } = DateTime.Now;

        public string AccountingYear { get; set; }   // Selected year
        public IEnumerable<SelectListItem> AccountingYearList { get; set; } // Dropdown list
        public int? TransId { get; set; }
        // Dropdown lists
        public IEnumerable<SelectListItem> Funds { get; set; }
        public IEnumerable<SelectListItem> SpecialAccounts { get; set; }
    }
}

