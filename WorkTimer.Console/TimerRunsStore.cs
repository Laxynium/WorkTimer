using System.IO.Abstractions;

namespace WorkTimer.Console;

public class TimerRunsStore
{
    private const string FileName = "workTimerRuns.json";
    private readonly AsyncLazy<IFileSystem> _fileSystem;

    public TimerRunsStore(AsyncLazy<IFileSystem> fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task Save(TimerRun timerRun)
    {
        await MakeSureStorageFileIsCreated();

        var content = await ReadFileContent();

        content.Items.Add(timerRun);

        await WriteFileContent(content);
    }

    private Task MakeSureStorageFileIsCreated() => Run(async fs =>
    {
        if (fs.File.Exists(FileName))
        {
            return;
        }

        await WriteFileContent(new TimerRuns());
    });

    private Task<TimerRuns> ReadFileContent() => Run(async fs =>
    {
        await using var readStream = fs.File.OpenRead(FileName);
        var result = await WorkTimerJsonSerializer.DeserializeAsync<TimerRuns>(readStream);
        return result!;
    });

    private Task WriteFileContent(TimerRuns content) => Run(async fs =>
    {
        await using var writeStream = fs.File.Create(FileName);
        await WorkTimerJsonSerializer.SerializeAsync(writeStream, content);
    });

    private async Task Run(Func<IFileSystem, Task> action) => await action(await _fileSystem.Value);

    private async Task<T> Run<T>(Func<IFileSystem, Task<T>> action) => await action(await _fileSystem.Value);

    private class TimerRuns
    {
        public List<TimerRun> Items { get; set; } = new();
    }
}