using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OAuthServices;
using OAuthServices.Data;
using OAuthServices.Middleware;
using OAuthServices.Services;
using System.Reflection;
using System.Text;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

AddSwagger(builder);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuração do banco de dados para logs
builder.Services.AddDbContext<LogContextDb>(options =>
    options.UseSqlServer(builder.Configuration["LogConnection"]), ServiceLifetime.Scoped);

builder.Services.AddDbContext<ContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

// Adiciona HttpContextAccessor para acessar o contexto HTTP
builder.Services.AddHttpContextAccessor();

// Configuração de logging com persistência no banco de dados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider()));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opts =>
    {
        opts.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(o => o.AddPolicy("spa",
    p => p.WithOrigins("http://webservices:8080")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()));

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseCors("spa");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservi�o de autentica��o e autoriza��o v1.0");
    });
}

app.Use(async (ctx, next) =>
{
    // Anti-CSRF (double-submit): se vier header, confira com cookie (opcional)
    // var xsrfHeader = ctx.Request.Headers["X-XSRF-TOKEN"].ToString();
    // var xsrfCookie = ctx.Request.Cookies["XSRF-TOKEN"];
    // if (ctx.Request.Method != "GET" && xsrfHeader != xsrfCookie) ctx.Response.StatusCode = 400;

    await next();
});

app.UseHttpsRedirection();

// Adiciona o middleware de Correlation ID no início do pipeline
app.UseCorrelationId();

// Configurar métricas HTTP do Prometheus
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("service", context => Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "oauth-service");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapMetrics(); // Endpoint /metrics - deve ser o último

app.Run();

static void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(s =>
    {
        s.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "OAuth",
            Description = "Microservi�o de autentica��o e autoriza��o - WebApi"
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

