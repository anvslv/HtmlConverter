
using HtmlConverter.ConversionService;
using HtmlConverter.Data;
using HtmlConverter.Hubs;
using HtmlConverter.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(cfg => cfg.EnableDetailedErrors = true) 
    .AddNewtonsoftJsonProtocol(opt => opt.PayloadSerializerSettings.Converters.Add(new StringEnumConverter()));  

builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

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
