var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHealthChecks();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("homeassistant", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://homeassistant.local:8123");
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();
