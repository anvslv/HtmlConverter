using HtmlConverter.Data;
using HtmlConverter.Data.Entities;
using HtmlConverter.Extensions;
using HtmlConverter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HtmlConverter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConverterJobsController : ControllerBase
{
    private readonly IHtmlConverterService _s; 
    private readonly ConversionJobsContext _ctx;

    public ConverterJobsController(IHtmlConverterService s, ConversionJobsContext ctx)
    {
        _s = s; 
        _ctx = ctx;
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
            return Ok();
        }
      
        return BadRequest("Could not save input html file"); 
    }

    [HttpGet]
    [Route("pdf")]
    public async Task<IActionResult> GetPdf()
    {
        byte[] pdfBytes = await _s.ConvertToPdfAsync();
        MemoryStream ms = new MemoryStream(pdfBytes);

        return new FileStreamResult(ms, "application/pdf");

    }
}
