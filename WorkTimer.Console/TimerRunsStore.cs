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

    public async Task<TimerRun> GetById(Guid id)
    {
        await MakeSureStorageFileIsCreated();

        var content = await ReadFileContent();

        return content.Items.Single(x => x.Id == id);
    }

    public async Task Add(TimerRun timerRun)
    {
        await MakeSureStorageFileIsCreated();

        var runs = await ReadFileContent();

        runs.Items.Add(timerRun);

        await WriteFileContent(runs);
    }

    public async Task Update(TimerRun timerRun)
    {
        await MakeSureStorageFileIsCreated();

        var runs = await ReadFileContent();

        var index = runs.Items.FindIndex(x => x.Id == timerRun.Id);
        if (index == -1)
        {
            runs.Items.Add(timerRun);
        }
        else
        {
            runs.Items[index] = timerRun;
        }

        await WriteFileContent(runs);
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