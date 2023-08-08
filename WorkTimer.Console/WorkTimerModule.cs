using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NodaTime;

namespace WorkTimer.Console;

public class WorkTimerModule
{
    private readonly TimerRunsStore _store;
    private readonly IClock _clock;
    private readonly ScriptsHooks _scriptsesHooks;

    public WorkTimerModule(TimerRunsStore store, IClock clock, ScriptsHooks scriptsesHooks)
    {
        _store = store;
        _clock = clock;
        _scriptsesHooks = scriptsesHooks;
    }

    public async Task<Guid> AddTimerRun(TimeLeft timeLeft, IReadOnlyDictionary<string, string>? labels = null)
    {
        labels ??= new Dictionary<string, string>();

        var now = _clock.GetCurrentInstant();

        var run = new TimerRun(Guid.NewGuid(), timeLeft.AsDuration, now, labels);

        await _store.Add(run);

        await _scriptsesHooks.InvokePreHooks(run);

        return run.Id;
    }

    public async Task CompleteTimerRun(Guid id)
    {
        var run = await _store.GetById(id);

        var now = _clock.GetCurrentInstant();

        run.Complete(now);

        await _store.Update(run);

        await _scriptsesHooks.InvokePostHooks(run);
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