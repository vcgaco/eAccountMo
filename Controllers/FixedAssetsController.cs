using eAccount.Data;
using eAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eAccount.Controllers
{


    public class FixedAssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixedAssetsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Load fixed assets with subsidiary accounts
            var assets = await _context.FixedAsset
                .Include(f => f.SubsidiaryAccount)
                .ToListAsync();

            return View(assets);
        }

        public IActionResult Create(int subsidiaryId)
        {
            Debug.WriteLine($"🔥 SubsidiaryId RECEIVED: {subsidiaryId}");
            var model = new FixedAsset
            {
                SubsidiaryAccountId = subsidiaryId
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FixedAsset asset)
        {
            Debug.WriteLine($"🔥 POST SubsidiaryAccountId = {asset.SubsidiaryAccountId}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"❌ {state.Key}: {error.ErrorMessage}");
                    }
                }

                return View(asset);
            }

            _context.FixedAsset.Add(asset);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "SubsidiaryAccounts");
        }



    }
}
