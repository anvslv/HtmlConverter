using PuppeteerSharp;

namespace HtmlConverter.ConversionService.HtmlToPdfConversion
{
    public interface IHtmlConverterService
    {
        Task<byte[]> ConvertToPdfAsync(string html);
    }

    public class HtmlConverterService : IHtmlConverterService
    {
        public async Task<byte[]> ConvertToPdfAsync(string html)
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            var pdfBytes = await page.PdfDataAsync();

            return pdfBytes;
        }
    }
}
