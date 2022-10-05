using HtmlConverter.Services;
using Microsoft.AspNetCore.Mvc;

namespace HtmlConverter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConverterJobsController : ControllerBase
{
    private readonly IHtmlConverterService _s;

    public ConverterJobsController(IHtmlConverterService s)
    {
        _s = s;
    }

    [HttpGet]
    [Route("jobs")]
    public IEnumerable<ConversionJob> GetJobs()
    {
        return new[]
        {
            new ConversionJob
            {
                ID = Guid.NewGuid(),
                HtmlFileName = "document-one.html",
                Status = ConversionStatus.Done
            }  ,
            new ConversionJob
            {
                ID = Guid.NewGuid(),
                HtmlFileName = "document-two.html",
                Status = ConversionStatus.InProgress
            }
        };
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
