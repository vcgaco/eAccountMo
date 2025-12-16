using eAccount.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace eAccount.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fund> Funds { get; set; }
        public DbSet<Jev> Jevs { get; set; }
        public DbSet<SpecialAccount> SpecialAccounts { get; set; }
        public DbSet<ChartofAccount> ChartofAccounts { get; set; }
        public DbSet<SubsidiaryAccount> SubsidiaryAccounts { get; set; }
        public DbSet<JevEntry> JevEntries { get; set; }
        public DbSet<SubsidiaryEntry> SubsidiaryEntries { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<FinancialReportClassification> FinancialReportClassifications { get; set; }
        public DbSet<FixedAsset> FixedAsset { get; set; }

    }

}
