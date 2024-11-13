using Microsoft.EntityFrameworkCore;
using GeneralValuationSubs.Models;
using System.Collections.Generic;

namespace ReadingTextFile.Data
{
    public class ApplicationAppContext:DbContext
    {
        public ApplicationAppContext(DbContextOptions<ApplicationAppContext> options) : base(options)
        {

        }

        public DbSet<FPL9Table> FPL9DBTable { get; set; }        
    }
}
