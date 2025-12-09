using eAccount.Data;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class AccountLedgerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountLedgerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(
            int? accountId,
            int? fundId,
            int? specialAccountId,
            DateTime? asOfDate)
        {
            var model = new AccountLedgerFilterViewModel
            {
                AccountId = accountId,
                FundId = fundId,
                SpecialAccountId = specialAccountId,
                AsOfDate = asOfDate ?? DateTime.Now,

                Accounts = await _context.ChartofAccounts
                    .OrderBy(a => a.AccountCode)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.AccountCode + " - " + a.AccountName
                    }).ToListAsync(),

                Funds = await _context.Funds
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.FundName
                    }).ToListAsync(),

                SpecialAccounts = await _context.SpecialAccounts
                    .Select(sa => new SelectListItem
                    {
                        Value = sa.Id.ToString(),
                        Text = sa.AccountName
                    }).ToListAsync()
            };

            // ✅ DO NOT LOAD ANY DATA UNTIL ACCOUNT IS SELECTED
            if (!accountId.HasValue)
                return View(model);

            var query = _context.JevEntries
                .Include(e => e.Jev)
                .Include(e => e.Account)
                .Include(e => e.Subsidiary)
                .Where(e => e.AccountId == accountId)
                .AsQueryable();

            if (fundId.HasValue)
                query = query.Where(e => e.Jev.FundId == fundId);

            if (specialAccountId.HasValue)
                query = query.Where(e => e.Jev.SpecialAccountId == specialAccountId);

            var endDate = new DateTime(model.AsOfDate.Year, model.AsOfDate.Month,
                DateTime.DaysInMonth(model.AsOfDate.Year, model.AsOfDate.Month));

            query = query.Where(e => e.Jev.EncodedDate <= endDate);

            model.LedgerEntries = await query
                .OrderBy(e => e.Jev.EncodedDate)
                .Select(e => new AccountLedgerEntry
                {
                    AccountCode = e.Account.AccountCode,
                    AccountName = e.Account.AccountName,
                    SubsidiaryName = e.Subsidiary != null ? e.Subsidiary.SubsidiaryName : null,
                    JevNo = e.Jev.JevNo,
                    EncodedDate = e.Jev.EncodedDate,
                    Particular = e.Jev.Particular,
                    Debit = e.Debit,
                    Credit = e.Credit
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetSpecialAccountsByFund(int fundId)
        {
            var specialAccounts = await _context.SpecialAccounts
                .Where(x => x.FundId == fundId)
                .Select(x => new
                {
                    id = x.Id,
                    text = x.AccountName
                })
                .ToListAsync();

            return Json(specialAccounts);
        }

    }
}

