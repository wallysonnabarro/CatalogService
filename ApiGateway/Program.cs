var builder = WebApplication.CreateBuilder(args);

// Configuração do Reverse Proxy (YARP)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Mapeia o Gateway
app.MapReverseProxy();

app.Run();
