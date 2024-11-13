using GeneralValuationSubs.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneralValuationSubs.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Draft> notValued { get; set; }
        public DbSet<FPL9Table> FPL9DBTable { get; set; }

        public DbSet<Journals_Audit> Journals_Audit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Journals_Audit>()
                .HasKey(j => j.Transaction_ID);  // Explicitly define the primary key
        }
    }
}
