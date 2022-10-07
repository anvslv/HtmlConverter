using Google.Protobuf;
using Grpc.Core;
using HtmlConverter.ConversionService.HtmlToPdfConversion;

namespace HtmlConverter.ConversionService.Services
{
    public class ConverterService : Conversion.ConversionBase
    {
        private readonly IHtmlConverterService _s;

        public ConverterService(IHtmlConverterService s)
        {
            _s = s;
        }

        public override async Task<ConvertReply> ConvertHtmlToPdf(ConvertRequest request, ServerCallContext context)
        {
            var bytes = await _s.ConvertToPdfAsync(request.Html);

            return new ConvertReply
            {
                Contents = ByteString.CopyFrom(bytes)
            };
        }
    }
}