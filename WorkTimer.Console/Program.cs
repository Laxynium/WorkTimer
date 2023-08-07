using System.Reactive.Concurrency;
using System.Reactive.Linq;
using SimpleExec;
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


var counterView = AnsiConsole
    .Live(new FigletText(string.Empty).Centered().Color(Color.Blue))
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
var text = $"Your {timeInMinutes}m work time is up";
AnsiConsole.Write(new FigletText(text).Centered().Color(Color.Green3));
AnsiConsole.Write(new FigletText("Good job for your hard and dedicated work").Centered());


try
{
    var (moduleName, _) = await Command.ReadAsync("pwsh",
        new[] { "-c", "Get-InstalledModule -Name BurntToast | %{$_.Name}" },
        handleExitCode: x => { return true; });

    if (!string.IsNullOrWhiteSpace(moduleName))
    {
        await Command.RunAsync("pwsh",
            new[]
            {
                "-c",
                @$"New-BurntToastNotification -Text ""{text}"", ""Take a break"", ""It was a great effort`nGood job for your hard`nand dedicated work"""
            },
            noEcho: true);
    }
    else
    {
        Console.WriteLine(
            "Looks like I cannot send you a system notification, since pwsh is missing BurntToast module");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Looks like I cannot send you a system notification, since there is no pwsh on your machine");
}