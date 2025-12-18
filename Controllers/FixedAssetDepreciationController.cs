using eAccount.Data;
using eAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eAccount.Controllers
{
    public class FixedAssetDepreciationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixedAssetDepreciationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // POST MONTHLY DEPRECIATION
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Run(DateTime accountingPeriod)
        {
            var assets = await _context.FixedAsset
             .Include(f => f.SubsidiaryAccount)
                 .ThenInclude(s => s.Account)
             .Where(f =>
                 f.Amount > 0 &&
                 f.UsefulLife > 0 &&
                 f.ScrapValue != null &&
                 f.DepreciationEffectivity != null &&
                 (f.DepreciationEffectivity.Value.Year < accountingPeriod.Year
                 || (f.DepreciationEffectivity.Value.Year == accountingPeriod.Year
                     && f.DepreciationEffectivity.Value.Month <= accountingPeriod.Month))
             )
             .ToListAsync();


            //var assets = await _context.FixedAsset
            //.Include(f => f.SubsidiaryAccount)
            //    .ThenInclude(s => s.Account) // ✅ correct
            //.Where(f =>
            //    f.Amount > 0 &&
            //    f.UsefulLife > 0 &&
            //    f.ScrapValue != null)
            //.ToListAsync();

            if (!assets.Any())
                return BadRequest("No depreciable assets found.");

            // ===============================
            // CREATE JEV HEADER
            // ===============================
            var lastDayOfMonth = new DateTime(accountingPeriod.Year, accountingPeriod.Month,
            DateTime.DaysInMonth(accountingPeriod.Year, accountingPeriod.Month));
            var jev = new Jev
            {
                JevNo = $"DEP-{accountingPeriod:yyyyMM}",
                AccountingPeriod = lastDayOfMonth,
                Particular = $"Monthly Depreciation {accountingPeriod:MMMM yyyy}",
                EncodedDate = DateTime.Now,
                AccountingYear = accountingPeriod.Year.ToString(),
                TransId = 4
            };

            _context.Jevs.Add(jev);
            await _context.SaveChangesAsync();

            decimal totalDepreciation = 0;

            foreach (var asset in assets)
            {
                var coa = asset.SubsidiaryAccount?.Account;

                if (coa == null ||
                    coa.DepreciationExpenseAccountId == null ||
                    coa.AccumulatedDepreciationAccountId == null)
                    continue;

                // ===============================
                // CALCULATE MONTHLY DEPRECIATION
                // ===============================
                var depreciableAmount = asset.Amount.Value - asset.ScrapValue.Value;
                var monthlyDep = depreciableAmount / (asset.UsefulLife.Value * 12);

                var remaining = asset.Amount.Value - asset.AccumulatedDepreciation;

                if (remaining - monthlyDep < asset.ScrapValue.Value)
                    monthlyDep = remaining - asset.ScrapValue.Value;

                if (monthlyDep <= 0)
                    continue;

                totalDepreciation += monthlyDep;

                // ===============================
                // SUBSIDIARY ENTRY (ACCUMULATED)
                // ===============================
                var subEntry = new SubsidiaryEntry
                {
                    SubsidiaryId = asset.SubsidiaryAccountId,
                    SubsidiaryCode = asset.SubsidiaryAccount.SubsidiaryCode,
                    SubsidiaryName = asset.SubsidiaryAccount.SubsidiaryName,
                    JevId = jev.Id,
                    Debit = 0,
                    Credit = monthlyDep
                };


                _context.SubsidiaryEntries.Add(subEntry);
                await _context.SaveChangesAsync();

                // ===============================
                // JEV ENTRY – DEPRECIATION EXPENSE
                // ===============================
                _context.JevEntries.Add(new JevEntry
                {
                    JevId = jev.Id,
                    AccountId = coa.DepreciationExpenseAccountId.Value,
                    SubsidiaryId = asset.SubsidiaryAccountId,
                    FixedAssetId = asset.Id,
                    Debit = monthlyDep,
                    Credit = 0
                });

                // ===============================
                // JEV ENTRY – ACCUMULATED DEP
                // ===============================
                _context.JevEntries.Add(new JevEntry
                {
                    JevId = jev.Id,
                    AccountId = coa.AccumulatedDepreciationAccountId.Value,
                    SubsidiaryId = asset.SubsidiaryAccountId,
                    FixedAssetId = asset.Id,
                    Debit = 0,
                    Credit = monthlyDep
                });

                // ===============================
                // FIXED ASSET ENTRY (HISTORY)
                // ===============================
                _context.FixedAssetEntries.Add(new FixedAssetEntry
                {
                    FixedAssetId = asset.Id,
                    SubsidiaryEntryId = subEntry.Id,
                    Credit = monthlyDep,
                    Debit = 0,
                    FixedAssetCode = asset.ChildCode
                });

                // ===============================
                // UPDATE FIXED ASSET BALANCES
                // ===============================
                asset.AccumulatedDepreciation += monthlyDep;
                asset.MonthlyDepreciation = monthlyDep;
                asset.AnnualDepreciation = monthlyDep * 12;
            }

            jev.JevAmount = totalDepreciation;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Jevs");
        }
    }
}
