using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddJsonFile("/app/options.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddHealthChecks();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("homeassistant", httpClient =>
{
    httpClient.BaseAddress = new Uri(configuration.GetValue("base_url", "http://supervisor/core/api/")!);
    string accessToken = configuration.GetValue("HASSIO_TOKEN", string.Empty)!;
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

app.MapGet("/config", (HttpContext context) =>
{
    var configDict = new Dictionary<string, string?>();
    foreach (var item in configuration.AsEnumerable())
    {
        configDict[item.Key] = item.Value;
    }
    return Results.Json(configDict);
});

// Endpoint para listar todos os arquivos no contêiner
app.MapGet("/files", async (HttpContext context) =>
{
    var rootDirectory = "/"; // Caminho raiz do contêiner
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
