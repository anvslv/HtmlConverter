using HtmlConverter;
using HtmlConverter.ConversionService;
using HtmlConverter.Data;
using HtmlConverter.Hubs;
using HtmlConverter.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
  
var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Settings").Get<Settings>();
 
builder.Services.AddCors(options => options
    .AddDefaultPolicy(policy => policy
        .WithOrigins(settings.ClientUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() 
    )
); 
builder.Services.AddSignalR(cfg => cfg.EnableDetailedErrors = true) 
    .AddNewtonsoftJsonProtocol(opt => opt.PayloadSerializerSettings.Converters.Add(new StringEnumConverter()));   
builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter())); 
builder.Services.AddHostedService<BackgroundConversionService>(); 
builder.Services.AddGrpcClient<Conversion.ConversionClient>(o => o
    .Address = new Uri(settings.ConversionServiceUrl));  
builder.Services
    .AddDbContextFactory<ConversionJobsContext>((s, b) => b
        .UseSqlite(settings.ConnectionString)
        .UseSnakeCaseNamingConvention()
        .UseLoggerFactory(
            s.GetRequiredService<ILoggerFactory>()));

var app = builder.Build();

app.Services.SeedData(); 
app.UseHttpsRedirection(); 
app.UseRouting();
app.UseCors(); 
app.UseAuthorization(); 
app.UseEndpoints(endpoints => endpoints
    .MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}"
    )
);
app.MapHub<ConverterHub>("/api/converterHub");
 
app.Run();
