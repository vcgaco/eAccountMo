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
    public class FinancialReportClassificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinancialReportClassificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FinancialReportClassifications
        public async Task<IActionResult> Index()
        {
            return View(await _context.FinancialReportClassifications.ToListAsync());
        }

        // GET: FinancialReportClassifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialReportClassification = await _context.FinancialReportClassifications
                .FirstOrDefaultAsync(m => m.id == id);
            if (financialReportClassification == null)
            {
                return NotFound();
            }

            return View(financialReportClassification);
        }

        // GET: FinancialReportClassifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FinancialReportClassifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,ReportType,Section,SubSection,Category,AccountCode,AccountName,DisplayOrder")] FinancialReportClassification financialReportClassification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(financialReportClassification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(financialReportClassification);
        }

        // GET: FinancialReportClassifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialReportClassification = await _context.FinancialReportClassifications.FindAsync(id);
            if (financialReportClassification == null)
            {
                return NotFound();
            }
            return View(financialReportClassification);
        }

        // POST: FinancialReportClassifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,ReportType,Section,SubSection,Category,AccountCode,AccountName,DisplayOrder")] FinancialReportClassification financialReportClassification)
        {
            if (id != financialReportClassification.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financialReportClassification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinancialReportClassificationExists(financialReportClassification.id))
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
            return View(financialReportClassification);
        }

        // GET: FinancialReportClassifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialReportClassification = await _context.FinancialReportClassifications
                .FirstOrDefaultAsync(m => m.id == id);
            if (financialReportClassification == null)
            {
                return NotFound();
            }

            return View(financialReportClassification);
        }

        // POST: FinancialReportClassifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var financialReportClassification = await _context.FinancialReportClassifications.FindAsync(id);
            if (financialReportClassification != null)
            {
                _context.FinancialReportClassifications.Remove(financialReportClassification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinancialReportClassificationExists(int id)
        {
            return _context.FinancialReportClassifications.Any(e => e.id == id);
        }
    }
}
