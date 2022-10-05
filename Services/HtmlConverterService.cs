using PuppeteerSharp;

namespace HtmlConverter.Services
{
    public interface IHtmlConverterService
    {
        Task<byte[]> ConvertToPdfAsync();
    }

    public class HtmlConverterService : IHtmlConverterService
    {
        public async Task<byte[]> ConvertToPdfAsync()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }); 
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync("<div>HTML to PDF</div>");
            var pdfBytes = await page.PdfDataAsync();

            return pdfBytes;
        }
    }
}
