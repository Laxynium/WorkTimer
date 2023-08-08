using System.Text.Json.Serialization;
using NodaTime;

namespace WorkTimer.Console;

public class TimerRun
{
    public Guid Id { get; }
    public Instant Timestamp { get; }
    public IReadOnlyDictionary<string, string> Labels { get; }

    [JsonConstructor]
    public TimerRun(Guid id, Instant timestamp, IReadOnlyDictionary<string, string> labels)
    {
        Id = id;
        Timestamp = timestamp;
        Labels = labels;
    }
}