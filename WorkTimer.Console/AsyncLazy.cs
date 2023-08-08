namespace WorkTimer.Console;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) : base(() => Task.FromResult(valueFactory()))
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory) : base(taskFactory)
    {
    }
}