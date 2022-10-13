using HtmlConverter.ConversionService.HtmlToPdfConversion;
using HtmlConverter.ConversionService.Services;

var builder = WebApplication.CreateBuilder(args);
  
builder.Services.AddGrpc();
builder.Services.AddSingleton<IHtmlConverterService, HtmlConverterService>();

var app = builder.Build();

app.MapGrpcService<ConverterService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. " +
                      "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
