using System.Text.Json.Serialization;
using HtmlConverter.ConversionService;
using HtmlConverter.Data;
using HtmlConverter.Hubs;
using HtmlConverter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(cfg => cfg.EnableDetailedErrors = true)
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });  

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });

builder.Services.AddHostedService<BackgroundConversionService>();

builder.Services.AddGrpcClient<Conversion.ConversionClient>(o =>
{
    o.Address = new Uri("https://localhost:7119");
});

builder.Services
    .AddDbContextFactory<ConversionJobsContext>((s, b) => b
        .UseSqlite("Data Source=Data/jobs.db")
        .UseSnakeCaseNamingConvention()
        .UseLoggerFactory(
            s.GetRequiredService<ILoggerFactory>()));

var app = builder.Build();

app.Services.SeedData();
 
if (!app.Environment.IsDevelopment())
{ 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHub<ConverterHub>("/api/converterHub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}"
    );
});

if (app.Environment.IsDevelopment())
{
    app.UseSpa(spa =>
    {
        spa.UseProxyToSpaDevelopmentServer("https://localhost:3000");
    });
}
else
{
    app.MapFallbackToFile("index.html");
}


app.Run();
