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
    public class ChartofAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartofAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChartofAccounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.ChartofAccounts
         .Include(x => x.Subsidiaries)
         .ToListAsync();

            return View(accounts);
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
            if (id == null)
            {
                return NotFound();
            }

            var chartofAccount = await _context.ChartofAccounts.FindAsync(id);
            if (chartofAccount == null)
            {
                return NotFound();
            }
            return View(chartofAccount);
        }

        // POST: ChartofAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountCode,AccountName,HasSubsidiary,Debit")] ChartofAccount chartofAccount)
        {
            if (id != chartofAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartofAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartofAccountExists(chartofAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chartofAccount);
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
    }
}
