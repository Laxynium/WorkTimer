using System.Text.Json.Serialization;
using NodaTime;

namespace WorkTimer.Console;

public class TimerRun
{
    public Guid Id { get; }
    public Duration Duration { get; }
    public Instant StartedAt { get; }
    public Instant? CompletedAt { get; private set; }
    public IReadOnlyDictionary<string, string> Labels { get; }

    [JsonConstructor]
    public TimerRun(Guid id, Duration duration, Instant startedAt, IReadOnlyDictionary<string, string> labels)
    {
        Id = id;
        Duration = duration;
        StartedAt = startedAt;
        Labels = labels;
    }

    public void Complete(Instant now)
    {
        CompletedAt = now;
    }
}