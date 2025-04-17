var builder = WebApplication.CreateBuilder(args);

// Add Swagger (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add YARP from config
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Logging middleware (for debug)
app.Use(async (context, next) =>
{
    Console.WriteLine($"[Gateway] → {context.Request.Method} {context.Request.Path}");
    await next();
});

// Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Optional HTTPS redirect (can remove in dev)
app.UseHttpsRedirection();

// YARP Gateway routing
app.MapReverseProxy();

app.Run();



