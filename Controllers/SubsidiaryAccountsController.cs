using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAccount.Data;
using eAccount.Models;

namespace eAccount.Controllers
{
    public class SubsidiaryAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubsidiaryAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubsidiaryAccounts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubsidiaryAccounts.Include(s => s.Account);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SubsidiaryAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subsidiaryAccount = await _context.SubsidiaryAccounts
                .Include(s => s.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subsidiaryAccount == null)
            {
                return NotFound();
            }

            return View(subsidiaryAccount);
        }

        // GET: SubsidiaryAccounts/Create
        public IActionResult Create()
        {
            ViewBag.Accounts = _context.ChartofAccounts
             .Where(x => x.HasSubsidiary)
             .ToList();

            return View();
        }

        // POST: SubsidiaryAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubsidiaryAccount model)
        {
            // ✅ Validate selected account really supports subsidiary
            var account = await _context.ChartofAccounts
                .FirstOrDefaultAsync(x => x.Id == model.AccountId);

            if (account == null)
                ModelState.AddModelError("", "Invalid Account.");

            else if (!account.HasSubsidiary)
                ModelState.AddModelError("", "This account does NOT allow subsidiaries.");

            if (!ModelState.IsValid)
            {
                ViewBag.Accounts = _context.ChartofAccounts
                    .Where(x => x.HasSubsidiary)
                    .ToList();

                return View(model);
            }

            _context.SubsidiaryAccounts.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ChartofAccounts");

        }

        // GET: SubsidiaryAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var sub = await _context.SubsidiaryAccounts.FindAsync(id);
            return View(sub);
        }

        // POST: SubsidiaryAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubsidiaryAccount model)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ChartofAccounts");
        }

        // GET: SubsidiaryAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var sub = await _context.SubsidiaryAccounts.FindAsync(id);
            return View(sub);
        }

        // POST: SubsidiaryAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sub = await _context.SubsidiaryAccounts.FindAsync(id);
            _context.SubsidiaryAccounts.Remove(sub);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ChartofAccounts");
        }

        private bool SubsidiaryAccountExists(int id)
        {
            return _context.SubsidiaryAccounts.Any(e => e.Id == id);
        }
        public async Task<IActionResult> ByAccount(int accountId)
        {
            var account = await _context.ChartofAccounts
                .Include(x => x.Subsidiaries)
                .FirstOrDefaultAsync(x => x.Id == accountId);

            if (account == null)
                return NotFound();

            return View(account);
        }
    }
}
