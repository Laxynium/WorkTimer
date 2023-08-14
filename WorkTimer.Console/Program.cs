using System.Reactive.Linq;
using SimpleExec;
using Spectre.Console;
using WorkTimer.Console;

var configuration = new WorkTimerModuleConfiguration();
var workTimerModule = configuration.Create();

var parsedInput = ParsedInput.Parse(args);

var runId = await workTimerModule.AddTimerRun(parsedInput.TimeLeft, parsedInput.Labels);

var keys = Observable.Defer(() => Observable.Start(Console.ReadKey))
    .Repeat()
    .Where(x => x.Key == ConsoleKey.S)
    .Select((_, i) =>
        i % 2 == 0
            ? new UiState(false, TimeLeft.Zero)
            : new UiState(true, TimeLeft.Zero));

var uiState = new UiState(true, parsedInput.TimeLeft);

var uiStateChanges = keys
    .StartWith(uiState)
    .Select(x => x.CountDown
        ? workTimerModule
            .GetCountdownTimer(uiState.TimeLeft)
            .Select(tl => uiState with { TimeLeft = tl })
            .Do(s => uiState = uiState with { TimeLeft = s.TimeLeft })
        : Observable.Never<UiState>()
            .StartWith(uiState with { CountDown = false }))
    .Switch()
    .TakeUntil(x => x.TimeLeft == TimeLeft.Zero);

await uiStateChanges.ForEachAsync(x =>
{
    if (x.CountDown)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText($"{x.TimeLeft}").Centered().Color(Color.Blue));
    }
    else
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText($"Paused").Centered().Color(Color.Blue));
        AnsiConsole.Write(new FigletText($"{x.TimeLeft}").Centered().Color(Color.Blue));
    }
});

await workTimerModule.CompleteTimerRun(runId);

var text = $"Your {parsedInput.TimeLeft} work time is up";

AnsiConsole.Clear();
AnsiConsole.Write(new FigletText(text).Centered().Color(Color.Green3));
AnsiConsole.Write(new FigletText("Good job for your hard and dedicated work").Centered());

try
{
    var (moduleName, _) = await Command.ReadAsync("pwsh",
        new[] { "-c", "'Get-InstalledModule -Name BurntToast | %{$_.Name}'" },
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

internal record UiState(bool CountDown, TimeLeft TimeLeft);