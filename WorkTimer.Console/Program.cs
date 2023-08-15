using System.Reactive.Linq;
using SimpleExec;
using Spectre.Console;
using WorkTimer.Console;

var configuration = new WorkTimerModuleConfiguration();
var workTimerModule = configuration.Create();

var parsedInput = ParsedInput.Parse(args);

var timerControl = Observable.Defer(() => Observable.Start(Console.ReadKey))
    .Repeat()
    .Where(x => x.Key == ConsoleKey.S)
    .Scan(true, (acc, _) => !acc);

var updateUi = ((TimeLeft timeLeft, bool isRunning) timer) =>
{
    if (timer.isRunning)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText($"{timer.timeLeft}").Centered().Color(Color.Blue));
    }
    else
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText($"Paused").Centered().Color(Color.Blue));
        AnsiConsole.Write(new FigletText($"{timer.timeLeft}").Centered().Color(Color.Blue));
    }
};

await workTimerModule.RunAsync(parsedInput.TimeLeft, parsedInput.Labels, timerControl, updateUi);

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