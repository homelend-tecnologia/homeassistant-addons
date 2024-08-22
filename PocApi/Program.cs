using InfluxDB.Client.Flux;

using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddJsonFile("/data/options.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddSingleton<IFluxClient>(options =>
{
    string url = configuration.GetValue("INFLUXDB_URL", "http://localhost:8086") ?? string.Empty;
    string username = configuration.GetValue("INFLUXDB_USERNAME", string.Empty) ?? string.Empty;
    string password = configuration.GetValue("INFLUXDB_PASSWORD", string.Empty) ?? string.Empty;
    var database = configuration.GetValue("INFLUXDB_DATABASE", string.Empty) ?? string.Empty;

    var connectionOptions = new FluxConnectionOptions(url, username, password.ToCharArray());
    return new FluxClient(connectionOptions);

});

/*
 * For InfluxDB 2.x
 * 
builder.Services.AddSingleton<IInfluxDBClient>(options =>
{
    var url = configuration.GetValue("INFLUXDB_URL", "http://localhost:8086");
    var username = configuration.GetValue("INFLUXDB_USERNAME", string.Empty);
    var password = configuration.GetValue("INFLUXDB_PASSWORD", string.Empty);
    var database = configuration.GetValue("INFLUXDB_DATABASE", string.Empty);

    return new InfluxDBClient(url,username, password, database, string.Empty);
});
*/

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("homeassistant", httpClient =>
{
    httpClient.BaseAddress = new Uri(configuration.GetValue("BASE_URL", "http://supervisor/core/api/")!);
    string accessToken = configuration.GetValue("HASSIO_TOKEN", string.Empty)!;
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
});
builder.Services.AddHttpClient("supervisor", httpClient =>
{
    httpClient.BaseAddress = new Uri(configuration.GetValue("BASE_URL", "http://supervisor/")!);
    string accessToken = configuration.GetValue("SUPERVISOR_TOKEN", string.Empty)!;
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

app.UseCors();

app.UseWebSockets(options: new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});

app.MapHealthChecks("/healthz");
app.MapControllers();

app.MapGet("/config", (HttpContext context) =>
{
    var configDict = new Dictionary<string, string?>();
    foreach (var item in configuration.AsEnumerable())
    {
        configDict[item.Key] = item.Value;
    }
    return Results.Json(configDict);
});

app.MapGet("/options", async (IHttpClientFactory httpClientFactory, HttpContext context) =>
{
    string addonSlug = configuration.GetValue("HOSTNAME", string.Empty) ?? string.Empty;
    addonSlug = "self";
    string supervisorApiUrl = $"/addons/{addonSlug}/options/config"; // caminho relativo

    var httpClient = httpClientFactory.CreateClient("supervisor");

    try
    {
        var response = await httpClient.GetAsync(supervisorApiUrl);
        response.EnsureSuccessStatusCode(); // Lança uma exceção se a resposta não for bem-sucedida

        var optionsJson = await response.Content.ReadAsStringAsync();
        var options = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(optionsJson);

        return Results.Json(optionsJson);
    }
    catch (Exception ex)
    {
        return Results.Json(ex);
    }
});

// Endpoint para listar todos os arquivos no contêiner
app.MapGet("/files/data", async (HttpContext context) =>
{
    var rootDirectory = "/data"; // Caminho raiz do contêiner
    var fileList = new List<string>();

    try
    {
        // Recursivamente obter todos os arquivos a partir do diretório raiz
        await Task.Run(() => GetFilesRecursively(rootDirectory, fileList));
        // Ordenar a lista de arquivos
        var sortedFileList = fileList.OrderBy(f => f).Select(f => new { Path = f }).ToList();
        return Results.Json(sortedFileList);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.MapGet("/files/app", async (HttpContext context) =>
{
    var rootDirectory = "/app"; // Caminho raiz do contêiner
    var fileList = new List<string>();

    try
    {
        // Recursivamente obter todos os arquivos a partir do diretório raiz
        await Task.Run(() => GetFilesRecursively(rootDirectory, fileList));
        // Ordenar a lista de arquivos
        var sortedFileList = fileList.OrderBy(f => f).Select(f => new { Path = f }).ToList();
        return Results.Json(sortedFileList);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

// Endpoint para obter um arquivo específico
app.MapGet("/files/data/{*path}", (HttpContext context) =>
{
    var path = context.Request.RouteValues["path"] as string;
    var fullPath = Path.Combine("/data", path!);

    if (!File.Exists(fullPath))
    {
        return Results.NotFound();
    }

    var fileStream = File.OpenRead(fullPath);
    return Results.File(fileStream, "application/octet-stream");
});

app.Run();



void GetFilesRecursively(string directory, List<string> fileList)
{
    try
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            fileList.Add(file);
        }

        foreach (var subDirectory in Directory.GetDirectories(directory))
        {
            GetFilesRecursively(subDirectory, fileList);
        }
    }
    catch (UnauthorizedAccessException)
    {
        // Ignorar diretórios aos quais não temos acesso
    }
    catch (Exception ex)
    {
        // Para fins de depuração, você pode registrar o erro aqui
        Console.WriteLine($"Erro ao acessar o diretório {directory}: {ex.Message}");
    }
}
