using eAccount.Data;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class FinancialPositionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinancialPositionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? fundId, int? specialAccountId, string period)
        {
            // ============================
            // DROPDOWNS
            // ============================
            ViewBag.Funds = await _context.Funds
                .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FundName })
                .ToListAsync();

            ViewBag.SpecialAccounts = fundId.HasValue
                ? await _context.SpecialAccounts
                    .Where(x => x.FundId == fundId)
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AccountName })
                    .ToListAsync()
                : new List<SelectListItem>();

            ViewBag.SelectedFund = fundId;
            ViewBag.SelectedSpecial = specialAccountId;
            ViewBag.Period = period ?? DateTime.Now.ToString("yyyy-MM");

            if (!fundId.HasValue)
                return View(new List<FinancialPositionViewModel>());

            // ============================
            // DATE RANGE
            // ============================
            DateTime startDate = DateTime.Parse(ViewBag.Period + "-01");
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // ============================
            // TRIAL BALANCE SOURCE
            // ============================
            var jevQuery = _context.JevEntries
                .Include(x => x.Jev)
                .Include(x => x.Account)
                .AsQueryable();

            jevQuery = jevQuery.Where(x =>
                x.Jev.FundId == fundId &&
                x.Jev.AccountingPeriod >= startDate &&
                x.Jev.AccountingPeriod <= endDate);

            if (specialAccountId.HasValue)
                jevQuery = jevQuery.Where(x => x.Jev.SpecialAccountId == specialAccountId);

            // ============================
            // BALANCES BY ACCOUNT CODE
            // ============================
            // Load balances first (filtered by fund / special / period)
            var balances = await _context.JevEntries
                .Include(j => j.Account)
                .Where(j => (!fundId.HasValue || j.Jev.FundId == fundId)
                         && (!specialAccountId.HasValue || j.Jev.SpecialAccountId == specialAccountId)
                         && j.Jev.AccountingPeriod <= endDate)
                .ToListAsync(); // Load to memory

            // Load classification manually (from FinancialReportClassification table)
            var classifications = await _context.FinancialReportClassifications
                .Where(c => c.ReportType == "SFP")
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // Now join in memory
            var report = classifications.Select(c =>
            {
                var accountBalance = balances
                    .Where(e => e.Account.AccountCode == c.AccountCode) // in-memory comparison is fine
                    .Sum(e => e.Debit - e.Credit);

                return new FinancialPositionViewModel
                {
                    Section = c.Section,
                    SubSection = c.SubSection,
                    AccountCode = c.AccountCode,
                    AccountName = c.AccountName,
                    Amount = accountBalance
                };
            }).ToList();


            return View(report);
        }


        // ✅ AJAX LOAD SPECIAL ACCOUNTS (IDENTICAL TO TRIAL BALANCE)
        public async Task<JsonResult> GetSpecialAccountsByFund(int fundId)
        {
            var accounts = await _context.SpecialAccounts
                .Where(sa => sa.FundId == fundId)
                .Select(sa => new
                {
                    id = sa.Id,
                    accountName = sa.AccountName
                })
                .ToListAsync();

            return Json(accounts);
        }
    }
}
