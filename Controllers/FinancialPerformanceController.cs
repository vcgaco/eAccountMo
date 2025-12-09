using eAccount.Data;
using eAccount.Models;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class FinancialPerformanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FinancialPerformanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? fundId, int? specialAccountId, string period)
        {
            // ✅ DROPDOWNS
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
            ViewBag.Period = period;

            // ✅ DO NOT LOAD UNTIL FILTER CLICKED
            if (!fundId.HasValue || string.IsNullOrEmpty(period))
                return View(new FinancialPerformanceViewModel());

            // ✅ DATE RANGE
            var startDate = DateTime.Parse(period + "-01");
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // ✅ LOAD ENTRIES
            var entries = _context.JevEntries
                .Include(x => x.Jev)
                .Include(x => x.Account)
                .AsQueryable();

            if (fundId.HasValue)
                entries = entries.Where(x => x.Jev.FundId == fundId);

            if (specialAccountId.HasValue)
                entries = entries.Where(x => x.Jev.SpecialAccountId == specialAccountId);

            entries = entries.Where(x =>
                x.Jev.AccountingPeriod >= startDate &&
                x.Jev.AccountingPeriod <= endDate);

            // ✅ GROUP TOTALS
            var rawData = await entries
                .GroupBy(x => new
                {
                    x.Account.AccountCode,
                    x.Account.AccountName,
                    x.Account.Debit   // ✅ NORMAL BALANCE
                })
                .Select(g => new
                {
                    g.Key.AccountCode,
                    g.Key.AccountName,
                    g.Key.Debit,
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit)
                })
                .ToListAsync();

            // ✅ FINAL COMPUTATION (IN MEMORY)
            var revenues = new List<FinancialPerformanceRow>();
            var expenses = new List<FinancialPerformanceRow>();

            foreach (var acc in rawData)
            {
                decimal amount;

                // ✅ NORMAL BALANCE LOGIC
                if (acc.Debit) // Normal DEBIT
                    amount = acc.TotalDebit - acc.TotalCredit;
                else           // Normal CREDIT
                    amount = acc.TotalCredit - acc.TotalDebit;

                var row = new FinancialPerformanceRow
                {
                    AccountCode = acc.AccountCode,
                    AccountName = acc.AccountName,
                    Amount = amount
                };

                // ✅ ✅ ✅ CLASSIFICATION BY ACCOUNT CODE
                if (acc.AccountCode.StartsWith("4"))
                    revenues.Add(row);

                if (acc.AccountCode.StartsWith("5"))
                    expenses.Add(row);
            }

            // ✅ FINAL VIEW MODEL
            var model = new FinancialPerformanceViewModel
            {
                Revenues = revenues,
                Expenses = expenses,
                TotalRevenue = revenues.Sum(x => x.Amount),
                TotalExpenses = expenses.Sum(x => x.Amount)
            };

            model.NetSurplus = model.TotalRevenue - model.TotalExpenses;

            return View(model);
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
