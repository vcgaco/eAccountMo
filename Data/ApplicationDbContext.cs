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
        public DbSet<FixedAssetEntry> FixedAssetEntries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Depreciation Expense: one Normal → many FA
            modelBuilder.Entity<ChartofAccount>()
                .HasOne(a => a.DepreciationExpenseAccount)
                .WithMany(a => a.FAUsingThisAsDepreciation)
                .HasForeignKey(a => a.DepreciationExpenseAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Accumulated Depreciation: one Accu → many FA
            modelBuilder.Entity<ChartofAccount>()
                .HasOne(a => a.AccumulatedDepreciationAccount)
                .WithMany(a => a.FAUsingThisAsAccumulated)
                .HasForeignKey(a => a.AccumulatedDepreciationAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
