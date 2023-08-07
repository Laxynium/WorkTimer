using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Spectre.Console;

var timeInMinutes = 90;
if (args.Length >= 1)
{
    timeInMinutes = Convert.ToInt32(args[0]);
}

var timeInSeconds = timeInMinutes * 60;

var countdownTimer = Observable
    .Timer(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1))
    .Take(timeInSeconds + 1)
    .ObserveOn(Scheduler.Default);


var counterView = AnsiConsole.Live(new FigletText("90").Centered().Color(Color.Blue))
    .StartAsync(async ctx =>
    {
        await countdownTimer
            .Select(x => x)
            .ForEachAsync(i =>
            {
                var minutes = (timeInSeconds - i) / 60;
                var seconds = (timeInSeconds - i) % 60;
                ctx.UpdateTarget(new FigletText($"{minutes:D2}:{seconds:D2}").Centered().Color(Color.Blue));
                ctx.Refresh();
            });
    });

await counterView;

AnsiConsole.Clear();
AnsiConsole.Write(new FigletText($"Your {timeInMinutes}m work time is up").Centered().Color(Color.Green3));