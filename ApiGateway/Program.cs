using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Data;
using ApiGateway.Middleware;
using ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

// Configuração do banco de dados para logs
builder.Services.AddDbContext<LogContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogConnection")), ServiceLifetime.Scoped);

// Adiciona HttpContextAccessor para acessar o contexto HTTP
builder.Services.AddHttpContextAccessor();

// Configuração de logging com persistência no banco de dados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider()));

// Registra o serviço de logging com Correlation ID
builder.Services.AddScoped<ICorrelationLogger, CorrelationLogger>();

builder.Services.AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10); // janela de 10s
        opt.PermitLimit = 5; // at� 5 requisi��es
        opt.QueueLimit = 2; // quantas ficam em fila
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

// Adiciona o middleware de Correlation ID no início do pipeline
app.UseCorrelationId();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use((context, next) =>
    {
        // Propaga o Correlation ID para os serviços downstream
        var correlationId = context.Items["CorrelationId"]?.ToString();
        if (!string.IsNullOrEmpty(correlationId))
        {
            context.Request.Headers["X-Correlation-ID"] = correlationId;
        }

        context.Response.GetTypedHeaders().CacheControl =
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(30) // 
            };
        return next();
    });
});

app.Run();
