using System.Reactive.Linq;
using SimpleExec;
using Spectre.Console;
using WorkTimer.Console;

var configuration = new WorkTimerModuleConfiguration();
var workTimerModule = configuration.Create();

var timeInMinutes = 90;
if (args.Length >= 1)
{
    timeInMinutes = Convert.ToInt32(args[0]);
}

IReadOnlyDictionary<string, string> labels = new Dictionary<string, string>();
if (args.Length >= 2)
{
    labels = new Dictionary<string, string>(args.Skip(1)
        .Select(x =>
        {
            var split = x.Split("=");
            return new KeyValuePair<string, string>(split[0], split[1]);
        }));
}

await workTimerModule.AddTimerRun(labels);

var timeLeft = TimeLeft.FromSeconds(timeInMinutes * 60);
var countdownTimer = workTimerModule.GetCountdownTimer(timeLeft);

var counterView = AnsiConsole
    .Live(new FigletText(string.Empty).Centered().Color(Color.Blue))
    .StartAsync(async ctx =>
    {
        await countdownTimer
            .ForEachAsync(t =>
            {
                ctx.UpdateTarget(new FigletText($"{t}").Centered().Color(Color.Blue));
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