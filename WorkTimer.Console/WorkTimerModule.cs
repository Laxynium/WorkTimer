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

    public async Task RunAsync(TimeLeft timeLeft, IReadOnlyDictionary<string, string> labels,
        IObservable<bool> timerControl, Action<(TimeLeft timeLeft, bool isRunning)> updateUi)
    {
        var runId = await AddTimerRun(timeLeft, labels);

        var countdownTimer = GetCountdownTimer(timeLeft, timerControl);

        await countdownTimer.ForEachAsync(updateUi);

        await CompleteTimerRun(runId);
    }

    private async Task<Guid> AddTimerRun(TimeLeft timeLeft, IReadOnlyDictionary<string, string>? labels = null)
    {
        labels ??= new Dictionary<string, string>();

        var now = _clock.GetCurrentInstant();

        var run = new TimerRun(Guid.NewGuid(), timeLeft.AsDuration, now, labels);

        await _store.Add(run);

        await _scriptsesHooks.InvokePreHooks(run);

        return run.Id;
    }

    private async Task CompleteTimerRun(Guid id)
    {
        var run = await _store.GetById(id);

        var now = _clock.GetCurrentInstant();

        run.Complete(now);

        await _store.Update(run);

        await _scriptsesHooks.InvokePostHooks(run);
    }

    private static IObservable<(TimeLeft timeLeft, bool isRunning)> GetCountdownTimer(TimeLeft timeLeft,
        IObservable<bool> timerControlStream)
    {
        var period = TimeSpan.FromSeconds(1);

        var result = timerControlStream
            .StartWith(true)
            .Select(x => x
                ? Observable.Timer(DateTimeOffset.UtcNow, period)
                    .Select(_ => "TICK")
                    .StartWith("START")
                : Observable.Never<string>()
                    .StartWith("STOP"))
            .Switch()
            .Scan((timeLeft: timeLeft.IncrementBySecond(), isRunning: true), (acc, @event) => @event switch
            {
                "START" => (acc.timeLeft, true),
                "STOP" => (acc.timeLeft, false),
                "TICK" => (acc.timeLeft.SubtractSeconds(1), acc.isRunning),
                _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null)
            })
            .Skip(1)
            .TakeUntil(x => x.timeLeft == TimeLeft.Zero);
        return result;
    }
}