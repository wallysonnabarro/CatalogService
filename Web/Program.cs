using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Services;
using Web.Middleware;
using Web;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LogContextDb>(options =>
    options.UseSqlServer(builder.Configuration["LogConnection"]), ServiceLifetime.Scoped);

builder.Services.AddHttpContextAccessor();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider()));

// Adicionar no builder.Services
builder.Services.AddHttpClient("webservices", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["apis:oauth"]!);
    client.DefaultRequestHeaders.Add("User-Agent", "webservices");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    return handler;
});

// HttpClient para CatalogService
builder.Services.AddHttpClient("gatewayservices", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["apis:gateway"]!);
    client.DefaultRequestHeaders.Add("User-Agent", "gatewayservices");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    return handler;
});

builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCorrelationId();

// Configurar métricas HTTP do Prometheus
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("service", context => Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "web-mvc");
});

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapMetrics(); // Endpoint /metrics - deve ser o último


app.Run();
