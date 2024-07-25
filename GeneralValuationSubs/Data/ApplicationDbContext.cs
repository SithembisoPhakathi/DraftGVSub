using GeneralValuationSubs.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneralValuationSubs.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Draft> notValued { get; set; }
    }
}
