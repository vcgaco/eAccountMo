using eAccount.Data;
using eAccount.Models;
using eAccount.Models.ViewModel;
using eAccount.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace eAccount.Controllers
{
    public class JevsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IAuditService _audit;
        public JevsController(ApplicationDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // GET: Jevs
        public async Task<IActionResult> Index(int? transId)
        {
            var jevs = _context.Jevs.AsQueryable();

            if (transId.HasValue)
            {
                jevs = jevs.Where(j => j.TransId == transId.Value);
                ViewData["TransId"] = transId.Value;
            }

            return View(await jevs.ToListAsync());
        }

        // GET: Jevs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jev = await _context.Jevs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jev == null)
            {
                return NotFound();
            }

            return View(jev);
        }

        // GET: Jevs/Create
        public async Task<IActionResult> Create(int? transId)
        {
            // Get Funds
            var funds = await _context.Funds
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.FundName
            }).ToListAsync();

            // Preload SpecialAccounts for the first Fund
            var firstFundId = funds.FirstOrDefault()?.Value != null ? int.Parse(funds.First().Value) : 0;
            var specialAccounts = await _context.SpecialAccounts
                .Where(sa => sa.FundId == firstFundId)
                .Select(sa => new SelectListItem
                {
                    Value = sa.Id.ToString(),
                    Text = sa.AccountName
                }).ToListAsync();



            var viewModel = new CreateJevViewModel
            {
                Funds = funds,
                SpecialAccounts = specialAccounts,
                FundId = firstFundId,
                SpecialAccountId = specialAccounts.FirstOrDefault() != null
                                    ? int.Parse(specialAccounts.First().Value)
                                    : (int?)null,

                TransId = transId ?? 0

            };

            // Generate past 10 years + current
            var currentYear = DateTime.Now.Year;

            viewModel.AccountingYearList = Enumerable.Range(currentYear - 9, 10)
                .Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                }).ToList();

            viewModel.AccountingYear = currentYear.ToString();

            return View(viewModel);
        }
        [HttpGet]
        public async Task<JsonResult> GetSpecialAccountsByFund(int fundId)
        {
            var accounts = await _context.SpecialAccounts
                .Where(sa => sa.FundId == fundId)
                .Select(sa => new { id = sa.Id, name = sa.AccountName })
                .ToListAsync();

            return Json(accounts);
        }

        // POST: Jevs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JevNo,JevAmount,Particular," +
            "EncodedDate," +
            "AccountingPeriod," +
            "FundId," +
            "SpecialAccountId," +
            "TransId," +
            "AccountingYear")] Jev jev)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jev);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jev);
        }

        // GET: Jevs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var jev = await _context.Jevs.FindAsync(id);
            if (jev == null)
                return NotFound();

            // ✅ ALWAYS LOAD FUNDS
            ViewBag.Funds = await _context.Funds
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.FundName
                })
                .ToListAsync();

            // ✅ LOAD SPECIAL ACCOUNTS BASED ON FUND
            ViewBag.SpecialAccounts = jev.FundId.HasValue
                ? await _context.SpecialAccounts
                    .Where(s => s.FundId == jev.FundId)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.AccountName
                    })
                    .ToListAsync()
                : new List<SelectListItem>();   // ✅ NEVER NULL

            return View(jev);
        }

        // POST: Jevs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Jev model)
        {
            Debug.WriteLine("🔥 EDIT POST HIT 🔥");
            Debug.WriteLine($"ID: {model.Id}");
            Debug.WriteLine($"FundId: {model.FundId}");
            Debug.WriteLine($"SpecialAccountId: {model.SpecialAccountId}");
            Debug.WriteLine($"Particular: {model.Particular}");
            Debug.WriteLine($"AccountingPeriod: {model.AccountingPeriod}");
            Debug.WriteLine($"Amount: {model.JevAmount}");
            // ✅ LOG MODELSTATE ERRORS
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("❌ MODELSTATE INVALID");

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"❌ FIELD: {state.Key} → {error.ErrorMessage}");
                    }
                }

                // ✅ Reload dropdowns
                ViewBag.Funds = await _context.Funds
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.FundName
                    })
                    .ToListAsync();

                ViewBag.SpecialAccounts = model.FundId.HasValue
                    ? await _context.SpecialAccounts
                        .Where(s => s.FundId == model.FundId)
                        .Select(s => new SelectListItem
                        {
                            Value = s.Id.ToString(),
                            Text = s.AccountName
                        })
                        .ToListAsync()
                    : new List<SelectListItem>();

                return View(model);
            }

            // ✅ FETCH EXISTING RECORD
            var jev = await _context.Jevs.FindAsync(model.Id);

            if (jev == null)
            {
                Debug.WriteLine("❌ JEV NOT FOUND IN DATABASE");
                return NotFound();
            }

            Debug.WriteLine("✅ JEV FOUND — UPDATING NOW");
            var year = model.AccountingPeriod.Year;
            var month = model.AccountingPeriod.Month;
            model.AccountingPeriod = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            // ✅ UPDATE
            jev.Particular = model.Particular;
            jev.AccountingPeriod = model.AccountingPeriod;
            jev.FundId = model.FundId;
            jev.SpecialAccountId = model.SpecialAccountId;
            jev.AccountingYear = model.AccountingYear;
            jev.JevAmount = model.JevAmount;
            try
            {
                await _context.SaveChangesAsync();
                Debug.WriteLine("✅ DATABASE SAVE SUCCESSFUL");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔥 DATABASE SAVE FAILED");
                Debug.WriteLine(ex.Message);
                throw;
            }

            Debug.WriteLine("✅ REDIRECTING TO DETAILS");

            return RedirectToAction("Details", new { id = jev.Id });
        }




        // GET: Jevs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jev = await _context.Jevs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jev == null)
            {
                return NotFound();
            }

            return View(jev);
        }

        // POST: Jevs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jev = await _context.Jevs.FindAsync(id);
            if (jev != null)
            {
                _context.Jevs.Remove(jev);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JevExists(int id)
        {
            return _context.Jevs.Any(e => e.Id == id);
        }

        // For Journal Entries

        public IActionResult JournalEntries(int id)
        {
            var jev = _context.Jevs
                .Include(j => j.Entries)
                    .ThenInclude(e => e.Account)
                .Include(j => j.Entries)
                    .ThenInclude(e => e.Subsidiary)
                .FirstOrDefault(j => j.Id == id);

            if (jev == null)
                return NotFound();

            return View(jev);
        }
        public IActionResult AddEntry(int jevId)
        {
            if (jevId == 0)
                return BadRequest("Invalid JEV ID");

            ViewBag.JevId = jevId;
            ViewBag.Accounts = _context.ChartofAccounts.ToList();
            ViewBag.Subsidiaries = _context.SubsidiaryAccounts.ToList();

            return View(new JevEntry { JevId = jevId });
        }

        // Save GL Journal Entries
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEntry(JevEntry model)
        {
            Debug.WriteLine("🔥 ADD ENTRY POST HIT 🔥");
            Debug.WriteLine($"JevId={model.JevId}, AccountId={model.AccountId}, SubsidiaryId={model.SubsidiaryId}, FixedAssetId={model.FixedAssetId}");

            // 🔹 Load account
            var account = await _context.ChartofAccounts
                .FirstOrDefaultAsync(x => x.Id == model.AccountId);

            // 🔹 Validate subsidiary requirement
            if (account?.HasSubsidiary == true && model.SubsidiaryId == null)
                ModelState.AddModelError("", "This account requires a subsidiary.");

            // 🔹 Validate Debit / Credit rules
            if (model.Debit > 0 && model.Credit > 0)
                ModelState.AddModelError("", "Only Debit OR Credit is allowed.");

            if (model.Debit == 0 && model.Credit == 0)
                ModelState.AddModelError("", "Debit or Credit is required.");

            // 🔹 Stop if invalid
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState.Values.SelectMany(v => v.Errors))
                    Debug.WriteLine("❌ MODEL ERROR: " + err.ErrorMessage);

                ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();
                return View(model);
            }

            // 🔹 Use transaction for safety
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // =========================================================
                // 1️⃣ SAVE JEV ENTRY
                // =========================================================
                _context.JevEntries.Add(model);
                await _context.SaveChangesAsync();

                Debug.WriteLine($"✅ JevEntry saved (ID={model.Id})");

                // =========================================================
                // 2️⃣ SAVE SUBSIDIARY ENTRY (IF ANY)
                // =========================================================
                SubsidiaryEntry subsidiaryEntry = null;

                if (model.SubsidiaryId != null)
                {
                    var subsidiary = await _context.SubsidiaryAccounts
                        .FirstOrDefaultAsync(x => x.Id == model.SubsidiaryId);

                    subsidiaryEntry = new SubsidiaryEntry
                    {
                        SubsidiaryId = subsidiary.Id,
                        SubsidiaryCode = subsidiary.SubsidiaryCode,
                        SubsidiaryName = subsidiary.SubsidiaryName,
                        JevId = model.JevId,
                        Debit = model.Debit,
                        Credit = model.Credit
                    };

                    _context.SubsidiaryEntries.Add(subsidiaryEntry);
                    await _context.SaveChangesAsync();

                    Debug.WriteLine($"✅ SubsidiaryEntry saved (ID={subsidiaryEntry.Id})");
                }

                // =========================================================
                // 3️⃣ SAVE FIXED ASSET ENTRY (IF SELECTED)
                // =========================================================
                if (model.FixedAssetId != null && subsidiaryEntry != null)
                {
                    var asset = await _context.FixedAsset
                        .FirstOrDefaultAsync(x => x.Id == model.FixedAssetId);

                    var assetEntry = new FixedAssetEntry
                    {
                        SubsidiaryEntryId = subsidiaryEntry.Id,
                        FixedAssetId = asset.Id,
                        FixedAssetCode = asset.ChildCode,
                        Debit = model.Debit,
                        Credit = model.Credit
                    };

                    _context.FixedAssetEntries.Add(assetEntry);
                    await _context.SaveChangesAsync();

                    Debug.WriteLine($"✅ FixedAssetEntry saved (ID={assetEntry.Id})");
                }

                // =========================================================
                // 4️⃣ COMMIT
                // =========================================================
                await transaction.CommitAsync();
                Debug.WriteLine("🎉 TRANSACTION COMMITTED");

                return RedirectToAction("JournalEntries", new { id = model.JevId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Debug.WriteLine("🔥 TRANSACTION FAILED: " + ex.Message);
                return BadRequest("Database error: " + ex.Message);
            }
        }

        // get subsidiaries
        [HttpGet]
        public async Task<JsonResult> GetSubsidiariesByAccount(int accountId)
        {

            var account = await _context.ChartofAccounts
        .Include(a => a.Subsidiaries)
        .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                return Json(new { requiresSubsidiary = false, subsidiaries = new List<object>() });

            var subs = account.Subsidiaries
                .Select(s => new
                {
                    id = s.Id,
                    subsidiaryCode = s.SubsidiaryCode,
                    subsidiaryName = s.SubsidiaryName
                }).ToList();

            return Json(new { requiresSubsidiary = account.HasSubsidiary, subsidiaries = subs });
        }

        // Edit Jev Entries
        [HttpGet]
        public async Task<IActionResult> EditEntry(int id)
        {
            // 1️⃣ Load JEV entry
            var entry = await _context.JevEntries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entry == null)
                return NotFound();

            var vm = new EditJevEntryVM
            {
                Id = entry.Id,
                JevId = entry.JevId,
                AccountId = entry.AccountId,
                SubsidiaryId = entry.SubsidiaryId,
                Debit = entry.Debit,
                Credit = entry.Credit
            };

            // 2️⃣ Load Accounts
            ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();

            // 3️⃣ Load Subsidiaries if applicable
            if (entry.SubsidiaryId != null)
            {
                // For dropdown
                ViewBag.Subsidiaries = await _context.SubsidiaryAccounts
                    .Where(x => x.AccountId == entry.AccountId)
                    .ToListAsync();

                // 4️⃣ Load SubsidiaryEntry
                var subsEntry = await _context.SubsidiaryEntries
                    .FirstOrDefaultAsync(x =>
                        x.JevId == entry.JevId &&
                        x.SubsidiaryId == entry.SubsidiaryId);

                if (subsEntry != null)
                {
                    // 5️⃣ Load FixedAssetEntry
                    var faEntry = await _context.FixedAssetEntries
                        .FirstOrDefaultAsync(x =>
                            x.SubsidiaryEntryId == subsEntry.Id);

                    if (faEntry != null)
                    {
                        vm.FixedAssetId = faEntry.FixedAssetId;

                        // 6️⃣ Load Fixed Assets dropdown
                        ViewBag.FixedAssets = await _context.FixedAsset
                            .Where(x => x.SubsidiaryAccountId == entry.SubsidiaryId)
                            .ToListAsync();
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEntry(EditJevEntryVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();
                return View(model);
            }

            var existingEntry = await _context.JevEntries
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (existingEntry == null)
                return NotFound();

            var newAccount = await _context.ChartofAccounts
                .FirstOrDefaultAsync(x => x.Id == model.AccountId);

            // ✅ Validate Debit/Credit
            if ((model.Debit <= 0 && model.Credit <= 0) || (model.Debit > 0 && model.Credit > 0))
            {
                ModelState.AddModelError("", "Only Debit OR Credit is allowed.");
                ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();
                return View(model);
            }

            // ✅ Subsidiary required check
            if (newAccount?.HasSubsidiary == true && model.SubsidiaryId == null)
            {
                ModelState.AddModelError("", "This account requires a subsidiary.");
                ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();
                return View(model);
            }

            // ✅ Start transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️⃣ Update main JEV Entry
                existingEntry.AccountId = model.AccountId;
                existingEntry.SubsidiaryId = model.SubsidiaryId;
                existingEntry.Debit = model.Debit;
                existingEntry.Credit = model.Credit;
                _context.JevEntries.Update(existingEntry);

                // 2️⃣ Handle SubsidiaryEntry
                var existingSubsEntry = await _context.SubsidiaryEntries
                    .FirstOrDefaultAsync(x => x.JevId == model.JevId && x.SubsidiaryId == existingEntry.SubsidiaryId);

                if (newAccount.HasSubsidiary && model.SubsidiaryId.HasValue)
                {
                    var sub = await _context.SubsidiaryAccounts.FindAsync(model.SubsidiaryId.Value);

                    if (existingSubsEntry == null)
                    {
                        existingSubsEntry = new SubsidiaryEntry
                        {
                            JevId = model.JevId,
                            SubsidiaryId = sub.Id,
                            SubsidiaryCode = sub.SubsidiaryCode,
                            SubsidiaryName = sub.SubsidiaryName,
                            Debit = model.Debit,
                            Credit = model.Credit
                        };
                        _context.SubsidiaryEntries.Add(existingSubsEntry);
                    }
                    else
                    {
                        existingSubsEntry.Debit = model.Debit;
                        existingSubsEntry.Credit = model.Credit;
                        existingSubsEntry.SubsidiaryId = sub.Id;
                        existingSubsEntry.SubsidiaryCode = sub.SubsidiaryCode;
                        existingSubsEntry.SubsidiaryName = sub.SubsidiaryName;
                        _context.SubsidiaryEntries.Update(existingSubsEntry);
                    }

                    // 3️⃣ Handle FixedAssetEntry
                    if (model.FixedAssetId.HasValue)
                    {
                        var existingFAEntry = await _context.FixedAssetEntries
                            .FirstOrDefaultAsync(fa => fa.SubsidiaryEntryId == existingSubsEntry.Id);

                        var fa = await _context.FixedAsset.FindAsync(model.FixedAssetId.Value);

                        if (existingFAEntry == null)
                        {
                            _context.FixedAssetEntries.Add(new FixedAssetEntry
                            {
                                SubsidiaryEntryId = existingSubsEntry.Id,
                                FixedAssetId = fa.Id,
                                FixedAssetCode = fa.ChildCode,
                                Debit = model.Debit,
                                Credit = model.Credit
                            });
                        }
                        else
                        {
                            existingFAEntry.FixedAssetId = fa.Id;
                            existingFAEntry.FixedAssetCode = fa.ChildCode;
                            existingFAEntry.Debit = model.Debit;
                            existingFAEntry.Credit = model.Credit;
                            _context.FixedAssetEntries.Update(existingFAEntry);
                        }
                    }
                    else
                    {
                        // Remove existing FixedAssetEntry if FixedAssetId cleared
                        var existingFAEntry = await _context.FixedAssetEntries
                            .FirstOrDefaultAsync(fa => fa.SubsidiaryEntryId == existingSubsEntry.Id);
                        if (existingFAEntry != null)
                            _context.FixedAssetEntries.Remove(existingFAEntry);
                    }
                }
                else
                {
                    // ✅ Delete orphan SubsidiaryEntry and connected FixedAssetEntry
                    if (existingSubsEntry != null)
                    {
                        var faEntry = await _context.FixedAssetEntries
                            .FirstOrDefaultAsync(fa => fa.SubsidiaryEntryId == existingSubsEntry.Id);
                        if (faEntry != null) _context.FixedAssetEntries.Remove(faEntry);

                        _context.SubsidiaryEntries.Remove(existingSubsEntry);
                    }
                    existingEntry.SubsidiaryId = null;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("JournalEntries", new { id = model.JevId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Error updating entry: " + ex.Message);
                ViewBag.Accounts = await _context.ChartofAccounts.ToListAsync();
                return View(model);
            }
        }



        [HttpGet]
        public async Task<JsonResult> GetSubsidiaries(int accountId)
        {
            var list = await _context.SubsidiaryAccounts
                .Where(x => x.AccountId == accountId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.SubsidiaryName
                })
                .ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> HasSubsidiary(int accountId)
        {
            var hasSub = await _context.ChartofAccounts
                .Where(x => x.Id == accountId)
                .Select(x => x.HasSubsidiary)
                .FirstOrDefaultAsync();

            return Json(hasSub);
        }
        // ✅ DELETE ENTRY (POST ONLY — SAFE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEntry(int id, int jevId)
        {
            var entry = await _context.JevEntries
         .FirstOrDefaultAsync(x => x.Id == id);

            if (entry == null)
                return NotFound();
            var oldData = new
            {
                entry.Id,
                entry.AccountId,
                entry.Debit,
                entry.Credit,
                entry.SubsidiaryId
            };

            await _audit.LogAsync(
                "JevEntries",
                entry.Id,
                "DELETE",
                oldData,
                null,
                User.Identity?.Name
            );
            // ✅ DELETE RELATED SUBSIDIARY ENTRY FIRST
            var subs = await _context.SubsidiaryEntries
                .Where(x => x.JevId == entry.JevId)
                .ToListAsync();

            _context.SubsidiaryEntries.RemoveRange(subs);

            // ✅ DELETE MAIN JEV ENTRY
            _context.JevEntries.Remove(entry);

            await _context.SaveChangesAsync();

            return RedirectToAction("JournalEntries", new { id = entry.JevId });
        }

        public async Task<IActionResult> PrintJev(int id)
        {
            var jev = await _context.Jevs
                .Include(j => j.Entries)
                    .ThenInclude(e => e.Account)
                .Include(j => j.Entries)
                    .ThenInclude(e => e.Subsidiary)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jev == null)
                return NotFound();

            return View(jev); // ✅ Create PrintJev.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> GetFixedAssetsBySubsidiary(int subsidiaryId)
        {
            var assets = await _context.FixedAsset
                .Where(x => x.SubsidiaryAccountId == subsidiaryId)
                .Select(x => new
                {
                    x.Id,
                    x.ChildCode,
                    x.ChildName
                })
                .ToListAsync();

            return Json(assets);
        }
    }
}
