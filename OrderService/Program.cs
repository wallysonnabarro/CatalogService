using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Aplicacao;
using OrderService.Data;
using OrderService.Servico;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

AddSwagger(builder);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ContextDb>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddScoped<IOrdemServicoUseCase, OrdemServicoUseCase>();
builder.Services.AddScoped<IOrdemServicoServices, OrdemServicoServices>();
builder.Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
builder.Services.AddScoped<IRabbitMqClient, RabbitMqClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "API Ordem de Serviços v1.0");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(s =>
    {
        s.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Ordem de Serviço",
            Description = "Microserviço de ordem de serviço - WebApi"
        });

        s.UseInlineDefinitionsForEnums();

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath))
        {
            s.IncludeXmlComments(xmlPath);
        }
    });
}
