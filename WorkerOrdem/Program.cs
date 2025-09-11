using MassTransit;
using Microsoft.EntityFrameworkCore;
using WorkerOrdem;
using WorkerOrdem.Data;
using WorkerOrdem.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<ContextDb>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

// Configuração do banco de dados para logs
builder.Services.AddDbContext<LogContextDb>(options =>
    options.UseSqlServer(builder.Configuration["LogConnection"]), ServiceLifetime.Scoped);

// Configuração de logging com persistência no banco de dados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider()));

builder.Services.AddSingleton<IRabbitMqReverterOrdem, RabbitMqReverterOrdem>();
builder.Services.AddSingleton<IProcessarEvento, ProcessarEvento>();
builder.Services.AddScoped<IOrdemRepository, OrdemRepository>();

var host = builder.Build();
host.Run();
