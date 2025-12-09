using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAccount.Models.ViewModel
{
    public class AccountLedgerFilterViewModel
    {
        // ✅ FILTERS
        public int? AccountId { get; set; }
        public int? FundId { get; set; }
        public int? SpecialAccountId { get; set; }
        public DateTime AsOfDate { get; set; } = DateTime.Now;

        // ✅ DROPDOWNS
        public List<SelectListItem> Accounts { get; set; } = new();
        public List<SelectListItem> Funds { get; set; } = new();
        public List<SelectListItem> SpecialAccounts { get; set; } = new();

        // ✅ RESULT
        public List<AccountLedgerEntry> LedgerEntries { get; set; } = new();
    }
}
