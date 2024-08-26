using InfluxDB.Client.Flux;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using System.Globalization;

namespace PocApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private readonly ILogger<HistoryController> _logger;
    private readonly IOptions<JsonOptions> _jsonOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFluxClient _fluxClient;
    private readonly string _influxBucket = "ha"; // TODO: trazer da configuração

    public HistoryController(ILogger<HistoryController> logger,
        IOptions<JsonOptions> jsonOptions,
        IHttpClientFactory httpClientFactory,
        IFluxClient fluxClient)
    {
        _logger = logger;
        _jsonOptions = jsonOptions;
        _httpClientFactory = httpClientFactory;
        _fluxClient = fluxClient;
    }

    ~HistoryController()
    {
        _fluxClient.Dispose();
    }


    [HttpGet]
    [Route("temperature/mean-test")]
    public async Task<IResult> GetTemperatureMeanTestAsync(
        [FromQuery] DateTimeOffset? start,
        [FromQuery] DateTimeOffset? stop,
        [FromQuery] List<string>? deviceList,
        [FromQuery] string interval = "1h"
        )
    {
        if (deviceList == null || deviceList.Count == 0)
        {
            return Results.BadRequest("deviceList is required");
        }

        string startFilter = start.HasValue ? start.Value.UtcDateTime.ToString("O") : "-24h";
        string stopFilter = stop.HasValue ? stop.Value.UtcDateTime.ToString("O") : "now()";
        
        decimal movingAverage = 3;
        string movingAverageStr = movingAverage.ToString(CultureInfo.InvariantCulture);

        var query = $@"
            getSensorQuery = (sensor_id) => {{
                return from(bucket: ""{_influxBucket}"")
                    |> range(start: {startFilter}, stop: {stopFilter})
                    |> filter(fn: (r) => r._measurement == ""°C"")
                    |> filter(fn: (r) => r._field == ""value"")
                    |> filter(fn: (r) => r.entity_id == sensor_id)
            }}
            applyMean = (tables=<-, interval) => {{
                return tables
                    |> aggregateWindow(every: interval, fn: mean, createEmpty: true)
                    |> keep(columns: [""_time"", ""_value""]) 
                    |> rename(columns: {{_value: ""mean""}})
            }}

            sensors = [{string.Join(", ", deviceList.Select(d => $"\"{d}\""))}]
            
            result = getSensorQuery(sensors[0]) |> applyMean(interval: {interval})

            for i from 1 to array.length(v: sensors) - 1 do
                result = join(tables: {{result: result, new: getSensorQuery(sensors[i]) |> applyMean(interval: {interval})}}, on: [""_time""])
                    |> map(fn: (r) => ({{
                        _time: r._time,
                        _value: r.mean + r.mean_1) / 2.0
                    }}))
        ";

        var tables = await _fluxClient.QueryAsync(query);

        // Lista para armazenar os resultados formatados
        var formattedResults = new List<dynamic>();

        return Results.Ok(formattedResults);

        //foreach (var table in tables)
        //{
        //    foreach (var row in table.Records)
        //    {
        //        formattedResults.Add(new
        //        {
        //            time = row.GetTime().ToString(),
        //            min = row.GetValueByKey("min"),
        //            max = row.GetValueByKey("max"),
        //            mean = row.GetValueByKey("mean")
        //        });
        //    }
        //}

        //return Results.Ok(formattedResults);

    }


    [HttpGet]
    [Route("temperature/mean/{deviceId:alpha}")]
    public async Task<IResult> GetTemperatureMeanByDeviceIdAsync(
        [FromRoute] string deviceId,
        [FromQuery] DateTimeOffset? start,
        [FromQuery] DateTimeOffset? stop,
        [FromQuery] string interval = "1h"
        )
    {
        string startFilter = start.HasValue ? start.Value.UtcDateTime.ToString("O") : "-24h";
        string stopFilter = stop.HasValue ? stop.Value.UtcDateTime.ToString("O") : "now()";

        var query = $@"
            base_query = from(bucket: ""{_influxBucket}"")
                |> range(start: {startFilter}, stop: {stopFilter})
                |> filter(fn: (r) => r._measurement == ""°C"")
                |> filter(fn: (r) => r._field == ""value"")
                |> filter(fn: (r) => r.entity_id == ""{deviceId}"")
            tmin = base_query 
                |> aggregateWindow(every: {interval}, fn: min, createEmpty: true)
                |> keep(columns: [""_time"", ""_value""]) 
                |> movingAverage(n: 3)
                |> rename(columns: {{_value: ""min""}})
            tmax = base_query 
                |> aggregateWindow(every: {interval}, fn: max, createEmpty: true)
                |> keep(columns: [""_time"", ""_value""]) 
                |> movingAverage(n: 3)
                |> rename(columns: {{_value: ""max""}})
            tmean = base_query 
                |> aggregateWindow(every: {interval}, fn: mean, createEmpty: true)
                |> keep(columns: [""_time"", ""_value""]) 
                |> movingAverage(n: 3)
                |> rename(columns: {{_value: ""mean""}})

            tminmax = join(tables: {{tmin: tmin, tmax: tmax}}, on: [""_time""])
            join(tables: {{tminmax: tminmax, tmean: tmean}}, on: [""_time""])
        ";

        var tables = await _fluxClient.QueryAsync(query);

        // Lista para armazenar os resultados formatados
        var formattedResults = new List<dynamic>();

        foreach (var table in tables)
        {
            foreach (var row in table.Records)
            {
                formattedResults.Add(new
                {
                    time = row.GetTime().ToString(),
                    min = row.GetValueByKey("min"),
                    max = row.GetValueByKey("max"),
                    mean = row.GetValueByKey("mean")
                });
            }
        }

        return Results.Ok(formattedResults);
    }

}
