using eAccount.Data;
using eAccount.Models;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class TrialBalanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrialBalanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        //public async Task<IActionResult> TrialBalance(int? fundId, int? specialAccountId, string period)
        //{
        //    // Load dropdowns
        //    var funds = await _context.Funds
        //        .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FundName })
        //        .ToListAsync();

        //    var specialAccounts = fundId.HasValue
        //        ? await _context.SpecialAccounts
        //            .Where(sa => sa.FundId == fundId.Value)
        //            .Select(sa => new SelectListItem { Value = sa.Id.ToString(), Text = sa.AccountName })
        //            .ToListAsync()
        //        : new List<SelectListItem>();

        //    ViewBag.Funds = funds;
        //    ViewBag.SpecialAccounts = specialAccounts;
        //    ViewBag.SelectedFund = fundId;
        //    ViewBag.SelectedSpecial = specialAccountId;
        //    ViewBag.Period = period ?? DateTime.Now.ToString("yyyy-MM");

        //    // Date range
        //    DateTime startDate, endDate;
        //    if (!string.IsNullOrEmpty(period))
        //    {
        //        startDate = DateTime.Parse(period + "-01");
        //        endDate = startDate.AddMonths(1).AddDays(-1);
        //    }
        //    else
        //    {
        //        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //        endDate = startDate.AddMonths(1).AddDays(-1);
        //    }

        //    // Query entries
        //    var entries = _context.JevEntries
        //        .Include(j => j.Jev)
        //        .Include(j => j.Account)
        //        .AsQueryable();

        //    if (fundId.HasValue)
        //        entries = entries.Where(j => j.Jev.FundId == fundId.Value);

        //    if (specialAccountId.HasValue)
        //        entries = entries.Where(j => j.Jev.SpecialAccountId == specialAccountId.Value);

        //    entries = entries.Where(j => j.Jev.AccountingPeriod >= startDate && j.Jev.AccountingPeriod <= endDate);

        //    var result = await entries
        //        .GroupBy(j => new { j.Account.AccountCode, j.Account.AccountName })
        //        .Select(g => new TrialBalanceViewModel
        //        {
        //            AccountCode = g.Key.AccountCode,
        //            AccountName = g.Key.AccountName,
        //            Debit = g.Sum(x => x.Debit),
        //            Credit = g.Sum(x => x.Credit)
        //        })
        //        .OrderBy(x => x.AccountCode)
        //        .ToListAsync();

        //    return View(result);
        //}
        public async Task<IActionResult> TrialBalance(int? fundId, int? specialAccountId, string period)
        {
            if (!fundId.HasValue && !specialAccountId.HasValue && string.IsNullOrEmpty(period))
            {
                ViewBag.Funds = await _context.Funds
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.FundName
                    })
                    .ToListAsync();

                ViewBag.SpecialAccounts = new List<SelectListItem>();
                ViewBag.SelectedFund = null;
                ViewBag.SelectedSpecial = null;
                ViewBag.Period = null;

                return View(new List<TrialBalanceViewModel>()); // ✅ EMPTY DATA
            }
            // ✅ Load dropdowns
            ViewBag.Funds = await _context.Funds
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.FundName
                })
                .ToListAsync();

            ViewBag.SpecialAccounts = fundId.HasValue
                ? await _context.SpecialAccounts
                    .Where(sa => sa.FundId == fundId.Value)
                    .Select(sa => new SelectListItem
                    {
                        Value = sa.Id.ToString(),
                        Text = sa.AccountName
                    })
                    .ToListAsync()
                : new List<SelectListItem>();

            ViewBag.SelectedFund = fundId;
            ViewBag.SelectedSpecial = specialAccountId;
            ViewBag.Period = period ?? DateTime.Now.ToString("yyyy-MM");

            // ✅ Date range from period
            DateTime startDate;
            DateTime endDate;

            if (!string.IsNullOrEmpty(period))
            {
                startDate = DateTime.Parse(period + "-01");
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            // ✅ Base query
            var entries = _context.JevEntries
                .Include(e => e.Jev)
                .Include(e => e.Account)
                .Where(e => e.Jev.AccountingPeriod >= startDate &&
                            e.Jev.AccountingPeriod <= endDate)
                .AsQueryable();

            if (fundId.HasValue)
                entries = entries.Where(e => e.Jev.FundId == fundId.Value);

            if (specialAccountId.HasValue)
                entries = entries.Where(e => e.Jev.SpecialAccountId == specialAccountId.Value);

            // ✅ Group & materialize FIRST (fixes expression tree error)
            var grouped = await entries
                .GroupBy(e => new
                {
                    e.Account.AccountCode,
                    e.Account.AccountName,
                    e.Account.Debit   // ✅ NORMAL BALANCE FLAG
                })
                .Select(g => new
                {
                    g.Key.AccountCode,
                    g.Key.AccountName,
                    NormalDebit = g.Key.Debit,
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit)
                })
                .OrderBy(x => x.AccountCode)
                .ToListAsync();   // ✅ VERY IMPORTANT

            // ✅ FINAL RESULT WITH NORMAL BALANCE + PARENTHESIS LOGIC
            var result = new List<TrialBalanceViewModel>();

            foreach (var g in grouped)
            {
                decimal debitValue = 0;
                decimal creditValue = 0;
                string debitDisplay = "";
                string creditDisplay = "";

                if (g.NormalDebit) // ✅ Normal balance is DEBIT
                {
                    var net = g.TotalDebit - g.TotalCredit;

                    if (net >= 0)
                    {
                        debitValue = net;
                        debitDisplay = net.ToString("N2");
                    }
                    else
                    {
                        debitValue = net; // keeps negative for TOTAL
                        debitDisplay = "(" + Math.Abs(net).ToString("N2") + ")";
                    }
                }
                else // ✅ Normal balance is CREDIT
                {
                    var net = g.TotalCredit - g.TotalDebit;

                    if (net >= 0)
                    {
                        creditValue = net;
                        creditDisplay = net.ToString("N2");
                    }
                    else
                    {
                        creditValue = net; // keeps negative for TOTAL
                        creditDisplay = "(" + Math.Abs(net).ToString("N2") + ")";
                    }
                }

                result.Add(new TrialBalanceViewModel
                {
                    AccountCode = g.AccountCode,
                    AccountName = g.AccountName,
                    Debit = debitValue,           // ✅ Numeric for TOTALS
                    Credit = creditValue,         // ✅ Numeric for TOTALS
                    DebitDisplay = debitDisplay,  // ✅ For UI with parentheses
                    CreditDisplay = creditDisplay
                });
            }

            return View(result);
        }


        public async Task<JsonResult> GetSpecialAccountsByFund(int fundId)
        {
            var accounts = await _context.SpecialAccounts
                .Where(sa => sa.FundId == fundId)
                .Select(sa => new { sa.Id, sa.AccountName })
                .ToListAsync();

            return Json(accounts);
        }
    }
}
