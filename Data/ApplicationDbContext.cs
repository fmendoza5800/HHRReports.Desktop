using Microsoft.EntityFrameworkCore;
using HHRReports.Desktop.Models;

namespace HHRReports.Desktop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Set command timeout to 20 minutes for long-running stored procedures
            this.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
        }

        public DbSet<PoolDetail> PoolDetails { get; set; }
        public DbSet<TerminalDetail> TerminalDetails { get; set; }
        public DbSet<PerformanceReport> PerformanceReports { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PoolDetail>().HasNoKey();
            modelBuilder.Entity<TerminalDetail>().HasNoKey();
            modelBuilder.Entity<PerformanceReport>().HasNoKey();
        }
    }
}
