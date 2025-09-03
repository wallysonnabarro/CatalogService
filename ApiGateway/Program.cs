var builder = WebApplication.CreateBuilder(args);

// Configura��o do Reverse Proxy (YARP)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Mapeia o Gateway
app.MapReverseProxy();

app.Run();
