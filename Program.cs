using HtmlConverter.Data; 
using HtmlConverter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IHtmlConverterService, HtmlConverterService>();

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
