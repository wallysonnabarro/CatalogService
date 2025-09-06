using CatalogService.Data;
using CatalogService.Middleware;
using CatalogService.Services;
using Microsoft.EntityFrameworkCore;
using OrderService;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

AddSwagger(builder);
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ContextDb>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

// Configuração do banco de dados para logs
builder.Services.AddDbContext<LogContextDb>(options =>
    options.UseSqlServer(builder.Configuration["LogConnection"]), ServiceLifetime.Scoped);

// Adiciona HttpContextAccessor para acessar o contexto HTTP
builder.Services.AddHttpContextAccessor();

// Configuração de logging com persistência no banco de dados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider()));

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "API Cat�logo de Produtos v1.0");
    });
}

app.UseHttpsRedirection();

// Adiciona o middleware de Correlation ID no início do pipeline
app.UseCorrelationId();

app.MapControllers();

app.Run();

static void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(s =>
    {
        s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Cat�logo de Produtos",
            Description = "Microservi�o de cat�logo de produtos - WebApi"
        });

        s.EnableAnnotations();
        s.UseInlineDefinitionsForEnums();

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath))
        {
            s.IncludeXmlComments(xmlPath);
        }
    });
}
