using System.IO.Abstractions;
using Spectre.Console;

namespace WorkTimer.Console;

public class ScriptsHooks
{
    private readonly AsyncLazy<IFileSystem> _fileSystem;

    public ScriptsHooks(AsyncLazy<IFileSystem> fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public Task InvokePreHooks(TimerRun timerRun) =>
        RunScripts(timerRun, "hooks/pre",
            (file, ex) =>
            {
                System.Console.WriteLine($"Failed to run a pre hook script {file.Name}");
                AnsiConsole.WriteException(ex);
            });

    public Task InvokePostHooks(TimerRun timerRun) =>
        RunScripts(timerRun, "hooks/post",
            (file, ex) =>
            {
                System.Console.WriteLine($"Failed to run a post hook script {file.Name}");
                AnsiConsole.WriteException(ex);
            });

    private async Task RunScripts(TimerRun timerRun, string location, Action<IFileInfo, Exception> onScriptError)
    {
        var fs = await _fileSystem.Value;
        var directory = fs.Directory.CreateDirectory(location);

        var scripts = directory.GetFiles("*.ps1");

        var serialized = WorkTimerJsonSerializer.SerializeToStringAsync(timerRun);

        foreach (var script in scripts)
        {
            try
            {
                await SimpleExec.Command.RunAsync("pwsh",
                    new[] { "-File", script.FullName, "-TimerRun", serialized },
                    noEcho: true);
            }
            catch (Exception e)
            {
                onScriptError.Invoke(script, e);
            }
        }
    }
}