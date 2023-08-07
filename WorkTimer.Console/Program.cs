using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Spectre.Console;

var timeInMinutes = 1; //Convert.ToInt32(args[1]);
var timeInSeconds = 10; //timeInMinutes * 60;

var countdownTimer = Observable
    .Timer(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1))
    .Take(timeInSeconds + 1)
    .ObserveOn(Scheduler.Default);


var counterView = AnsiConsole.Live(new FigletText("90"))
    .StartAsync(async ctx =>
    {
        await countdownTimer
            .Select(x => x)
            .ForEachAsync(i =>
            {
                ctx.UpdateTarget(new FigletText($"{timeInSeconds - i}"));
                ctx.Refresh();
            });
    });

await counterView;