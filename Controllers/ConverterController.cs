using Microsoft.AspNetCore.Mvc;

namespace HtmlConverter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConverterJobsController : ControllerBase
{ 
    public ConverterJobsController( )
    { 
    }

    [HttpGet]
    public IEnumerable<ConversionJob> Get()
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
}
