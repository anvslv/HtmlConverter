using HtmlConverter.Data;
using HtmlConverter.Data.Entities;
using HtmlConverter.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HtmlConverter.Services;

public class BackgroundConversionService : BackgroundService
{ 
    private readonly IDbContextFactory<ConversionJobsContext> _cf;
    private readonly IHtmlConverterService _s;
    private readonly IHubContext<ConverterHub, IConverterHub> _hc;
    private const int Delay = 1 * 10 * 1000; // 10 seconds

    public BackgroundConversionService(IDbContextFactory<ConversionJobsContext> cf, IHtmlConverterService s, IHubContext<ConverterHub, IConverterHub> hc)
    { 
        _cf = cf;
        _s = s;
        _hc = hc;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Delay, stoppingToken);
            await DoConversionAsync();
        }
    }

    private async Task DoConversionAsync()
    {
        using var ctx = await _cf.CreateDbContextAsync();

        var pendingJobs = ctx.Jobs.Where(x => x.Status == ConversionStatus.ReceivedInputFile).AsAsyncEnumerable();
             
        await foreach (var job in pendingJobs)
        {
            job.Status = ConversionStatus.InProgress;
            await ctx.SaveChangesAsync(); 
            await _hc.Clients.All.ConversionStatusChanged(job.ID, ConversionStatus.InProgress);

            var pdfBytes = await _s.ConvertToPdfAsync(job.HtmlContents);
            job.PdfContents = pdfBytes;
            job.Status = ConversionStatus.Done;
            await ctx.SaveChangesAsync(); 
            await _hc.Clients.All.ConversionStatusChanged(job.ID, ConversionStatus.Done);  
        } 
    }
}