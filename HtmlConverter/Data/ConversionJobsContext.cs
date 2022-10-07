using HtmlConverter.Data.Entities; 
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HtmlConverter.Data
{ 
    public class ConversionJobsContext : DbContext
    { 
        public ConversionJobsContext(DbContextOptions options) : base(options)
        {
        }
         
        public DbSet<ConversionJob> Jobs { get; set; } = default!; 
    }
}
