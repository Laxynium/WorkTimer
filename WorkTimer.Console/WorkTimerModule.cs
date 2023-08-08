using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NodaTime;

namespace WorkTimer.Console;

public class WorkTimerModule
{
    private readonly TimerRunsStore _store;
    private readonly IClock _clock;

    public WorkTimerModule(TimerRunsStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    public async Task AddTimerRun(IReadOnlyDictionary<string, string>? labels = null)
    {
        labels ??= new Dictionary<string, string>();

        var now = _clock.GetCurrentInstant();

        var run = new TimerRun(Guid.NewGuid(), now, labels);

        await _store.Save(run);
    }

    public IObservable<TimeLeft> GetCountdownTimer(TimeLeft timeLeft)
    {
        var countdownTimer = Observable
            .Timer(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1))
            .Take(timeLeft.InSeconds + 1)
            .Select(timeLeft.SubtractSeconds)
            .ObserveOn(Scheduler.Default);
        return countdownTimer;
    }
}