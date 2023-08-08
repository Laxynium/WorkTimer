using CSharpFunctionalExtensions;
using NodaTime;
using Superpower;

namespace WorkTimer.Console;

public class TimeLeft : ValueObject
{
    private readonly int _value;

    private TimeLeft(int value)
    {
        _value = value;
    }

    public static TimeLeft FromSeconds(int value) => new(value);

    public static TimeLeft FromString(string str) => UsingParser(str);

    private TimeLeft Add(TimeLeft timeLeft) => new(InSeconds + timeLeft.InSeconds);

    private static TimeLeft UsingParser(string str)
    {
        var hours = Superpower.Parsers.Span.Regex("[0-9]+h")
            .Select(x => FromSeconds(Convert.ToInt32(x.ToString().TrimEnd('h')) * 60 * 60));

        var minutes = Superpower.Parsers.Span.Regex("[0-9]+m")
            .Select(x => FromSeconds(Convert.ToInt32(x.ToString().TrimEnd('m')) * 60));

        var seconds = Superpower.Parsers.Span.Regex("[0-9]+s")
            .Select(x => FromSeconds(Convert.ToInt32(x.ToString().TrimEnd('s'))));

        var minutesNoUnit = Superpower.Parsers.Span.Regex("[0-9]+")
            .Select(x => FromSeconds(Convert.ToInt32(x.ToString()) * 60));

        var hoursMinutesSeconds = hours.Then(h => minutes.Select(h.Add)).Then(m => seconds.Select(m.Add));
        var hoursMinutes = hours.Then(h => minutes.Select(h.Add));
        var minutesSeconds = minutes.Then(m => seconds.Select(m.Add));
        var hoursSeconds = hours.Then(h => seconds.Select(h.Add));

        var parser = hoursMinutesSeconds.AtEnd().Try()
            .Or(hoursMinutes.AtEnd().Try())
            .Or(hoursSeconds.AtEnd().Try())
            .Or(minutesSeconds.AtEnd().Try())
            .Or(hours.AtEnd().Try())
            .Or(minutes.AtEnd().Try())
            .Or(seconds.AtEnd().Try())
            .Or(minutesNoUnit.AtEnd().Try());


        var result = parser.TryParse(str);
        if (!result.HasValue)
        {
            throw new InvalidOperationException("Invalid work time format");
        }

        return result.Value;
    }

    public int InSeconds => _value;
    public Duration AsDuration => Duration.FromSeconds(InSeconds);

    public TimeLeft SubtractSeconds(long seconds) => FromSeconds(InSeconds - (int)seconds);

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString()
    {
        var hours = _value / (60 * 60);
        var remainingSeconds = _value % 3600;
        var minutes = remainingSeconds / 60;
        var seconds = remainingSeconds % 60;

        var hoursFormat = hours == 0 ? string.Empty : $"{hours:D2}:";
        var minutesFormat = hours == 0 && minutes == 0 ? string.Empty : $"{minutes:D2}:";

        return $"{hoursFormat}{minutesFormat}{seconds:D2}s";
    }
}