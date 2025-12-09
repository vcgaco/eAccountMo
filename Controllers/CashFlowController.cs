using eAccount.Data;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class CashFlowController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashFlowController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? fundId, int? specialAccountId, string period)
        {
            // ✅ Load dropdowns
            ViewBag.Funds = await _context.Funds
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.FundName
                }).ToListAsync();

            ViewBag.SpecialAccounts = fundId.HasValue
                ? await _context.SpecialAccounts
                    .Where(sa => sa.FundId == fundId)
                    .Select(sa => new SelectListItem
                    {
                        Value = sa.Id.ToString(),
                        Text = sa.AccountName
                    }).ToListAsync()
                : new List<SelectListItem>();

            ViewBag.SelectedFund = fundId;
            ViewBag.SelectedSpecial = specialAccountId;
            ViewBag.Period = period;

            // ✅ Do NOT load until filter is clicked
            if (!fundId.HasValue || string.IsNullOrEmpty(period))
                return View(null);

            // ✅ Date Range
            var startDate = DateTime.Parse(period + "-01");
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // ✅ Only CASH accounts (1-01-01-xxx)
            var entries = _context.JevEntries
                .Include(x => x.Jev)
                .Include(x => x.Account)
                .Where(x => x.Jev.AccountingPeriod >= startDate &&
                            x.Jev.AccountingPeriod <= endDate &&
                            x.Account.AccountCode.StartsWith("1-01")) // ✅ CASH ONLY
                .AsQueryable();

            if (fundId.HasValue)
                entries = entries.Where(x => x.Jev.FundId == fundId);

            if (specialAccountId.HasValue)
                entries = entries.Where(x => x.Jev.SpecialAccountId == specialAccountId);

            var rawCash = await entries
                .GroupBy(x => new
                {
                    x.Account.AccountCode,
                    x.Account.AccountName,
                    x.Account.Debit // ✅ NORMAL BALANCE
                })
                .Select(g => new
                {
                    g.Key.AccountCode,
                    g.Key.AccountName,
                    g.Key.Debit,
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit)
                }).ToListAsync(); // ✅ MUST MATERIALIZE FIRST

            var operating = new List<CashFlowItem>();
            var investing = new List<CashFlowItem>();
            var financing = new List<CashFlowItem>();

            foreach (var x in rawCash)
            {
                decimal net = x.Debit
                    ? x.TotalDebit - x.TotalCredit
                    : x.TotalCredit - x.TotalDebit;

                // ✅ LGU Classification
                if (x.AccountCode.StartsWith("1-01"))
                    operating.Add(new CashFlowItem { AccountCode = x.AccountCode, AccountName = x.AccountName, Amount = net });

                else if (x.AccountCode.StartsWith("1-02"))
                    investing.Add(new CashFlowItem { AccountCode = x.AccountCode, AccountName = x.AccountName, Amount = net });

                else if (x.AccountCode.StartsWith("3"))
                    financing.Add(new CashFlowItem { AccountCode = x.AccountCode, AccountName = x.AccountName, Amount = net });
            }

            var model = new CashFlowViewModel
            {
                OperatingActivities = operating,
                InvestingActivities = investing,
                FinancingActivities = financing,
                NetOperating = operating.Sum(x => x.Amount),
                NetInvesting = investing.Sum(x => x.Amount),
                NetFinancing = financing.Sum(x => x.Amount)
            };

            model.NetIncreaseInCash =
                model.NetOperating +
                model.NetInvesting +
                model.NetFinancing;

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
