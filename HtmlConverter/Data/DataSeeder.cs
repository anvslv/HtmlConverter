using HtmlConverter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HtmlConverter.Data;

public static class DataSeeder
{
    public static void SeedData(this IServiceProvider sp)
    {
        var factory = sp.GetRequiredService<IDbContextFactory<ConversionJobsContext>>();
        var context = factory.CreateDbContext();

        if (!context.Database.EnsureCreated())
        {
            return;
        }

        var job = new ConversionJob
        {
            ID = 1,
            HtmlFileName = "document-one.html",
            HtmlContents = "<div>HTML to PDF</div>",
            Status = ConversionStatus.ReceivedInputFile
        };
             
        var job2 = new ConversionJob
        {
            ID = 2,
            HtmlFileName = "document-two.html",
            HtmlContents = "<div>HTML to PDF 2</div>", 
            Status = ConversionStatus.ReceivedInputFile
        };

        context.Jobs.Add(job);  
        context.Jobs.Add(job2); 
        context.SaveChanges();
    }
}