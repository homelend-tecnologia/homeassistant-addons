using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddHealthChecks();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("homeassistant", httpClient =>
{
    httpClient.BaseAddress = new Uri(configuration.GetValue("BASE_URL","http://homeassistant.local:8123")!);
    string accessToken = configuration.GetValue("ACCESS_TOKEN", string.Empty)!;
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

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
