using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAccount.Data;
using eAccount.Models;
using System.Diagnostics;
namespace eAccount.Controllers
{
    public class ChartofAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartofAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChartofAccounts
        public async Task<IActionResult> Index(string search)
        {
            //   var accounts = await _context.ChartofAccounts
            //.Include(x => x.Subsidiaries)
            //.ToListAsync();

            //   return View(accounts);
            var accounts = _context.ChartofAccounts.AsQueryable();

            // ✅ SEARCH FILTER
            if (!string.IsNullOrEmpty(search))
            {
                accounts = accounts.Where(a =>
                    a.AccountCode.Contains(search) ||
                    a.AccountName.Contains(search));
            }

            ViewBag.Search = search;

            return View(await accounts
                .OrderBy(a => a.AccountCode)
                .ToListAsync());
        }

        // GET: ChartofAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartofAccount = await _context.ChartofAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartofAccount == null)
            {
                return NotFound();
            }

            return View(chartofAccount);
        }

        // GET: ChartofAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChartofAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountCode,AccountName,HasSubsidiary,Debit")] ChartofAccount chartofAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartofAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chartofAccount);
        }

        // GET: ChartofAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var chartofAccount = await _context.ChartofAccounts
                .Include(a => a.DepreciationExpenseAccount)
                .Include(a => a.AccumulatedDepreciationAccount)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (chartofAccount == null) return NotFound();

            await PopulateDropdowns(chartofAccount);

            return View(chartofAccount);
        }




        // POST: ChartofAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountCode,AccountName,HasSubsidiary,Debit,AccountType,DepreciationExpenseAccountId,AccumulatedDepreciationAccountId")] ChartofAccount chartofAccount)
        {
            if (id != chartofAccount.Id) return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateDropdowns(chartofAccount);
                    return View(chartofAccount);
                }

                // Attach entity and mark modified
                _context.Attach(chartofAccount);
                _context.Entry(chartofAccount).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------- POST Edit Exception ----------");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);

                // Debug posted values
                Console.WriteLine("ChartOfAccount Data:");
                Console.WriteLine($"Id: {chartofAccount.Id}");
                Console.WriteLine($"AccountCode: {chartofAccount.AccountCode}");
                Console.WriteLine($"AccountName: {chartofAccount.AccountName}");
                Console.WriteLine($"AccountType: {chartofAccount.AccountType}");
                Console.WriteLine($"DepreciationExpenseAccountId: {chartofAccount.DepreciationExpenseAccountId}");
                Console.WriteLine($"AccumulatedDepreciationAccountId: {chartofAccount.AccumulatedDepreciationAccountId}");

                await PopulateDropdowns(chartofAccount);

                ModelState.AddModelError("", "An error occurred while saving. Check console/debug output.");
                return View(chartofAccount);
            }
        }



        // GET: ChartofAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartofAccount = await _context.ChartofAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartofAccount == null)
            {
                return NotFound();
            }

            return View(chartofAccount);
        }

        // POST: ChartofAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chartofAccount = await _context.ChartofAccounts.FindAsync(id);
            if (chartofAccount != null)
            {
                _context.ChartofAccounts.Remove(chartofAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartofAccountExists(int id)
        {
            return _context.ChartofAccounts.Any(e => e.Id == id);
        }
        private async Task PopulateDropdowns(ChartofAccount chartofAccount)
        {
            var depreciationAccounts = await _context.ChartofAccounts
                .Where(a => a.AccountType == AccountType.Normal)
                .OrderBy(a => a.AccountCode)
                .ToListAsync();

            ViewBag.DepreciationAccounts = new SelectList(
                depreciationAccounts,
                "Id",
                "AccountName",
                chartofAccount.DepreciationExpenseAccountId
            );

            var accumulatedAccounts = await _context.ChartofAccounts
                .Where(a => a.AccountType == AccountType.Accu)
                .OrderBy(a => a.AccountCode)
                .ToListAsync();

            ViewBag.AccumulatedAccounts = new SelectList(
                accumulatedAccounts,
                "Id",
                "AccountName",
                chartofAccount.AccumulatedDepreciationAccountId
            );
        }


    }
}
