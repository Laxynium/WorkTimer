using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;

namespace WorkTimer.Console;

public class WorkTimerModuleConfiguration
{
    private readonly IServiceCollection _services;

    private static readonly string AppHomeDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WorkTimer");

    public WorkTimerModuleConfiguration()
    {
        _services = new ServiceCollection();
        _services.TryAddSingleton<AsyncLazy<IFileSystem>>(_ => new AsyncLazy<IFileSystem>(() =>
        {
            IFileSystem fs = new FileSystem();
            fs.Directory.CreateDirectory(AppHomeDirectory);
            fs.Directory.SetCurrentDirectory(AppHomeDirectory);
            return fs;
        }));
        _services.AddSingleton<IClock>(SystemClock.Instance);
        _services.AddSingleton<TimerRunsStore>();
        _services.AddSingleton<WorkTimerModule>();

        _services.AddSingleton<ScriptsHooks>();
    }

    public WorkTimerModule Create()
    {
        var sp = _services.BuildServiceProvider(new ServiceProviderOptions
            { ValidateOnBuild = true, ValidateScopes = true });

        return sp.GetRequiredService<WorkTimerModule>();
    }
}