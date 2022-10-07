﻿using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using HtmlConverter.Data;
using HtmlConverter.Data.Entities;
using HtmlConverter.Hubs;
using HtmlConverter.ConversionService;

namespace HtmlConverter.Services;

public class BackgroundConversionService : BackgroundService
{
    private readonly IDbContextFactory<ConversionJobsContext> _cf;
    private readonly Conversion.ConversionClient _c;
    private readonly IHubContext<ConverterHub, IConverterHub> _hc;
    private readonly ILogger<BackgroundConversionService> _logger;
    private const int Delay = 1 * 10 * 1000;

    public BackgroundConversionService(
        IDbContextFactory<ConversionJobsContext> cf, 
        Conversion.ConversionClient c,
        IHubContext<ConverterHub, IConverterHub> hc,
        ILogger<BackgroundConversionService> logger)
    {
        _cf = cf;
        _c = c;
        _hc = hc;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Delay, stoppingToken);
            await DoConversionAsync();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BackgroundConversionService stopping...");

        await using var ctx = await _cf.CreateDbContextAsync();

        var pendingJobs = ctx.Jobs
            .Where(x => x.Status == ConversionStatus.InProgress)
            .AsAsyncEnumerable();

        await foreach (var job in pendingJobs)
        {
            job.Status = ConversionStatus.ReceivedInputFile;
            await ctx.SaveChangesAsync();
        }

        _logger.LogInformation("BackgroundConversionService stopped");
    }

    private async Task DoConversionAsync()
    {
        await using var ctx = await _cf.CreateDbContextAsync();

        var pendingJobs = ctx.Jobs
            .Where(x => x.Status == ConversionStatus.ReceivedInputFile || 
                        x.Status == ConversionStatus.Failed_ConnectionServiceUnavailable)
            .AsAsyncEnumerable();

        await foreach (var job in pendingJobs)
        {
            job.Status = ConversionStatus.InProgress;
            await ctx.SaveChangesAsync();
            await _hc.Clients.All.ConversionStatusChanged(job.ID, ConversionStatus.InProgress);

            try
            {
                var convertReply = await _c.ConvertHtmlToPdfAsync(new ConvertRequest
                {
                    Html = job.HtmlContents
                }); 
           
                job.PdfContents = convertReply.ToByteArray();
                job.Status = ConversionStatus.Done;
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Unavailable)
                {
                    job.Status = ConversionStatus.Failed_ConnectionServiceUnavailable; 
                } 
            }
             
            await ctx.SaveChangesAsync();
            await _hc.Clients.All.ConversionStatusChanged(job.ID, job.Status.Value);
        }
    }
}