using eAccount.Data;
using eAccount.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

public class SubsidiaryMonitoringController : Controller
{
    private readonly ApplicationDbContext _context;

    public SubsidiaryMonitoringController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? accountId, int? subsidiaryId, DateTime? asOfDate)
    {
        // Load only accounts that have subsidiaries
        ViewBag.Accounts = await _context.ChartofAccounts
            .Where(x => x.HasSubsidiary)
            .ToListAsync();

        if (accountId == null)
            return View(new List<SubsidiaryMonitoringVM>());

        var account = await _context.ChartofAccounts
            .Include(x => x.Subsidiaries)
            .FirstOrDefaultAsync(x => x.Id == accountId);

        ViewBag.Subsidiaries = account?.Subsidiaries;
        ViewBag.SelectedAccount = accountId;
        ViewBag.SelectedSubsidiary = subsidiaryId;

        if (subsidiaryId == null)
            return View(new List<SubsidiaryMonitoringVM>());

        asOfDate ??= DateTime.Today;
        ViewBag.AsOfDate = asOfDate.Value.ToString("yyyy-MM-dd");

        // Fetch SubsidiaryEntries for selected subsidiary up to the specified date
        var entries = await _context.SubsidiaryEntries
            .Include(x => x.Jev)
            .Where(x => x.SubsidiaryId == subsidiaryId && x.Jev.EncodedDate <= asOfDate)
            .OrderBy(x => x.Jev.EncodedDate)
            .ToListAsync();

        decimal balance = 0;

        var data = entries.Select(x =>
        {
            // Calculate running balance: Debit - Credit
            balance += x.Debit - x.Credit;

            return new SubsidiaryMonitoringVM
            {
                Date = x.Jev.EncodedDate,
                JevNo = x.Jev.JevNo,
                Particular = x.Jev.Particular,
                Debit = x.Debit,
                Credit = x.Credit,
                Balance = balance
            };
        }).ToList();

        return View(data);
    }
    public async Task<IActionResult> GetSubsidiariesByAccount(int accountId)
    {
        var subs = await _context.SubsidiaryAccounts
            .Where(s => s.AccountId == accountId)
            .Select(s => new { s.Id, s.SubsidiaryCode, s.SubsidiaryName })
            .ToListAsync();
        return Json(subs);
    }

}
