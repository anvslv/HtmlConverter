using HtmlConverter.Data;
using HtmlConverter.Data.Entities;
using HtmlConverter.Extensions;
using HtmlConverter.Hubs;
using HtmlConverter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HtmlConverter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConverterJobsController : ControllerBase
{
    private readonly ConversionJobsContext _ctx;
    private readonly IHubContext<ConverterHub, IConverterHub> _c;

    public ConverterJobsController(ConversionJobsContext ctx, IHubContext<ConverterHub, IConverterHub> c)
    {
        _ctx = ctx;
        _c = c;
    }

    [HttpGet]
    [Route("jobs")]
    public async Task<IEnumerable<ConversionJob>> GetJobs()
    {
        return await _ctx.Jobs.ToListAsync();
    }

    [HttpPost]
    [Route("createJob")]
    public async Task<IActionResult> CreateJob(IFormFile file)
    {
        if (file.FileName.EndsWith(".html") == false)
        {
            return BadRequest("Please submit HTML file"); 
        }
         
        var j = new ConversionJob()
        {
            HtmlFileName = file.FileName,
            HtmlContents = await file.ReadAsStringAsync(),
            Status = ConversionStatus.ReceivedInputFile
        };

        await _ctx.AddAsync(j);

        var savingResult = await _ctx.SaveChangesAsync();

        if (savingResult > 0)
        {
            await _c.Clients.All.NewConversionJob(j);

            return Ok();
        }
      
        return BadRequest("Could not save input html file"); 
    }

    [HttpGet] 
    [Route("pdf/{id}")]
    public async Task<IActionResult> GetPdf(int id)
    {
        var x = await _ctx.Jobs.Where(x => x.ID == id).FirstOrDefaultAsync();

        if (x == null)
        {
            return BadRequest($"Could not get PDF with id {id}: job not found");
        }

        if (x.Status != ConversionStatus.Done)
        {
            return BadRequest($"Could not get PDF with id {id}: job not finished yet");
        }

        if (x.PdfContents == null)
        {
            return BadRequest($"Could not get PDF with id {id}: failed to get pdf contents");
        }
         
        MemoryStream ms = new MemoryStream(x.PdfContents);

        return new FileStreamResult(ms, "application/pdf");
        
    }
}
