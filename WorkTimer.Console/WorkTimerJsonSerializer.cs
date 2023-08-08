using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace WorkTimer.Console;

public class WorkTimerJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    public static string SerializeToStringAsync<T>(T content) =>
        JsonSerializer.Serialize(content, options: Options);


    public static Task SerializeAsync<T>(Stream stream, T content) =>
        JsonSerializer.SerializeAsync(stream, content, options: Options);

    public static ValueTask<T?> DeserializeAsync<T>(Stream stream)
    {
        return JsonSerializer.DeserializeAsync<T>(stream, options: Options);
    }
}