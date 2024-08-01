using PocApi.Converters.Json;

using System.Text.Json.Serialization;

namespace PocApi.Models.HomeAssistant.State;

public class Attributes
{
    // Common Properties
    [JsonPropertyName("editable")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? Editable { get; set; }
    [JsonPropertyName("friendly_name")]
    public string? FriendlyName { get; set; }
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
    [JsonPropertyName("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; }
    [JsonPropertyName("device_class")]
    public string? DeviceClass { get; set; }
    [JsonPropertyName("entity_picture")]
    public string? EntityPicture { get; set; }
    [JsonPropertyName("supported_features")]
    public int? SupportedFeatures { get; set; }

    // Specific Properties (examples from the provided response)
    [JsonPropertyName("auto_update")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? AutoUpdate { get; set; }
    [JsonPropertyName("installed_version")]
    public string? InstalledVersion { get; set; }
    [JsonPropertyName("in_progress")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? InProgress { get; set; }
    [JsonPropertyName("latest_version")]
    public string? LatestVersion { get; set; }
    [JsonPropertyName("release_summary")]
    public string? ReleaseSummary { get; set; }
    [JsonPropertyName("release_url")]
    public string? ReleaseUrl { get; set; }
    [JsonPropertyName("skipped_version")]
    public string? SkippedVersion { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("supported_color_modes")]
    public List<string>? SupportedColorModes { get; set; }
    [JsonPropertyName("color_mode")]
    public string? ColorMode { get; set; }
    [JsonPropertyName("effect_list")]
    public List<string>? EffectList { get; set; }
    [JsonPropertyName("effect")]
    public string? Effect { get; set; }
    [JsonPropertyName("raw_state")]
    [JsonConverter(typeof(StringAllowAllValuesConverter))]
    public string? RawState { get; set; }
    [JsonPropertyName("state_class")]
    public string? StateClass { get; set; }
    [JsonPropertyName("last_reset")]
    public DateTime? LastReset { get; set; }
    [JsonPropertyName("backups")]
    public List<Backup>? Backups { get; set; }
    [JsonPropertyName("free_space_in_google_drive")]
    public string? FreeSpaceInGoogleDrive { get; set; }
    [JsonPropertyName("source_type")]
    public string? SourceType { get; set; }
    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }
    [JsonPropertyName("min")]
    public double? Min { get; set; }
    [JsonPropertyName("max")]
    public double? Max { get; set; }
    [JsonPropertyName("step")]
    public double? Step { get; set; }
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
    [JsonPropertyName("radius")]
    public int? Radius { get; set; }
    [JsonPropertyName("passive")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? Passive { get; set; }
    [JsonPropertyName("persons")]
    public List<object>? Persons { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("device_trackers")]
    public List<object>? DeviceTrackers { get; set; }
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    [JsonPropertyName("frontend_stream_type")]
    public string? FrontendStreamType { get; set; }
    [JsonPropertyName("next_dawn")]
    public DateTime? NextDawn { get; set; }
    [JsonPropertyName("next_dusk")]
    public DateTime? NextDusk { get; set; }
    [JsonPropertyName("next_midnight")]
    public DateTime? NextMidnight { get; set; }
    [JsonPropertyName("next_noon")]
    public DateTime? NextNoon { get; set; }
    [JsonPropertyName("next_rising")]
    public DateTime? NextRising { get; set; }
    [JsonPropertyName("next_setting")]
    public DateTime? NextSetting { get; set; }
    [JsonPropertyName("elevation")]
    public double? Elevation { get; set; }
    [JsonPropertyName("azimuth")]
    public double? Azimuth { get; set; }
    [JsonPropertyName("rising")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? Rising { get; set; }
    [JsonPropertyName("last_triggered")]
    public string? LastTriggered { get; set; }
    [JsonPropertyName("current")]
    public int? Current { get; set; }
    [JsonPropertyName("restored")]
    [JsonConverter(typeof(BooleanWithoutQuotesConverter))]
    public bool? Restored { get; set; }
    [JsonPropertyName("last_backup")]
    public DateTime? LastBackup { get; set; }
    [JsonPropertyName("next_backup")]
    public DateTime? NextBackup { get; set; }
    [JsonPropertyName("last_uploaded")]
    public DateTime? LastUploaded { get; set; }
    [JsonPropertyName("backups_in_google_drive")]
    public int? BackupsInGoogleDrive { get; set; }
    [JsonPropertyName("backups_in_home_assistant")]
    public int? BackupsInHomeAssistant { get; set; }
    [JsonPropertyName("size_in_google_drive")]
    public string? SizeInGoogleDrive { get; set; }
    [JsonPropertyName("size_in_home_assistant")]
    public string? SizeInHomeAssistant { get; set; }
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }
    [JsonPropertyName("dew_point")]
    public double? DewPoint { get; set; }
    [JsonPropertyName("temperature_unit")]
    public string? TemperatureUnit { get; set; }
    [JsonPropertyName("humidity")]
    public int? Humidity { get; set; }
    [JsonPropertyName("cloud_coverage")]
    public double? CloudCoverage { get; set; }
    [JsonPropertyName("pressure")]
    public double? Pressure { get; set; }
    [JsonPropertyName("pressure_unit")]
    public string? PressureUnit { get; set; }
    [JsonPropertyName("wind_bearing")]
    public double? WindBearing { get; set; }
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }
    [JsonPropertyName("wind_speed_unit")]
    public string? WindSpeedUnit { get; set; }
    [JsonPropertyName("visibility_unit")]
    public string? VisibilityUnit { get; set; }
    [JsonPropertyName("precipitation_unit")]
    public string? PrecipitationUnit { get; set; }
    [JsonPropertyName("attribution")]
    public string? Attribution { get; set; }
}

