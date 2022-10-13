using Google.Protobuf;
using Grpc.Core;
using HtmlConverter.ConversionService.HtmlToPdfConversion;

namespace HtmlConverter.ConversionService.Services
{
    public class ConverterService : Conversion.ConversionBase
    {
        private readonly IHtmlConverterService _s;
        private readonly ILogger<ConverterService> _l;

        public ConverterService(IHtmlConverterService s, ILogger<ConverterService> l)
        {
            _s = s;
            _l = l;
        }

        public override async Task<ConvertReply> ConvertHtmlToPdf(ConvertRequest request, ServerCallContext context)
        {
            _l.LogInformation($"Started conversion: ${request.Filename}");
            var bytes = await _s.ConvertToPdfAsync(request.Html);
            _l.LogInformation($"Finished conversion: ${request.Filename}");

            return new ConvertReply
            {
                Contents = ByteString.CopyFrom(bytes)
            };
        }
    }
}