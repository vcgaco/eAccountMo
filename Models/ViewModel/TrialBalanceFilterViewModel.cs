using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAccount.Models.ViewModel
{
    public class TrialBalanceFilterViewModel
    {
        public int? FundId { get; set; }
        public int? SpecialAccountId { get; set; }
        public string Period { get; set; } // yyyy-MM

        public List<SelectListItem> Funds { get; set; }
        public List<SelectListItem> SpecialAccounts { get; set; }
    }
}
