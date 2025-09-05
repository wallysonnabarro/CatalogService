using Microsoft.EntityFrameworkCore;
using WorkerCatalog;
using WorkerCatalog.Data;
using WorkerCatalog.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


builder.Services.AddDbContext<ContextDb>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddScoped<IProdutosRepository, ProdutosRepository>();
builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
builder.Services.AddSingleton<IProcessarEvento, ProcessarEvento>();


var host = builder.Build();
host.Run();
