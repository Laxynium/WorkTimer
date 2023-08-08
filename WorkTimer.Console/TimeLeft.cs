using CSharpFunctionalExtensions;

namespace WorkTimer.Console;

public class TimeLeft : ValueObject
{
    private readonly int _value;

    private TimeLeft(int value)
    {
        _value = value;
    }

    public static TimeLeft FromSeconds(int value) => new(value);

    public static TimeLeft FromString(string str)
    {
        var value = Convert.ToInt32(str);
        return new TimeLeft(value);
    }

    public int InSeconds => _value;

    public TimeLeft SubtractSeconds(long seconds)  => FromSeconds(InSeconds - (int)seconds);

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
    {
        var minutes = _value / 60;
        var seconds = _value % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
}