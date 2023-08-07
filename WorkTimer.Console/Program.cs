using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Spectre.Console;

var timeInMinutes = 1; //Convert.ToInt32(args[1]);
var timeInSeconds = 10;//timeInMinutes * 60;

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

// var progress = AnsiConsole.Progress()
//     .AutoRefresh(false)
//     .Columns(new ProgressBarColumn(), new PercentageColumn());

// var progressBar = progress
//     .StartAsync(ctx =>
//     {
//         var task1 = ctx.AddTask("[green] Starting to do something[/]");
//
//         var subscribe = countdownTimer.Subscribe(x =>
//         {
//             task1.Increment(100D / 60);
//             ctx.Refresh();
//         });
//
//         while (!ctx.IsFinished)
//         {
//         }
//
//         subscribe.Dispose();
//         return Task.CompletedTask;
//     });
//
// await Task.WhenAll(counterView, progressBar);