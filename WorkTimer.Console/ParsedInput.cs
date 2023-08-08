namespace WorkTimer.Console;

public class ParsedInput
{
    private static readonly TimeLeft DefaultTimeLeft = TimeLeft.FromSeconds(90 * 60);
    public TimeLeft TimeLeft { get; }
    public IReadOnlyDictionary<string, string> Labels { get; }

    public ParsedInput(TimeLeft timeLeft, IReadOnlyDictionary<string, string> labels)
    {
        TimeLeft = timeLeft;
        Labels = labels;
    }

    public static ParsedInput Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new ParsedInput(DefaultTimeLeft, new Dictionary<string, string>());
        }

        var timeLeft = TimeLeft.FromString(args[0]);

        var labels = new Dictionary<string, string>(args.Skip(1).Select(ParseLabel));

        return new ParsedInput(timeLeft, labels);
    }

    private static KeyValuePair<string, string> ParseLabel(string value)
    {
        var split = value.Split("=");
        if (split.Length != 2)
        {
            throw new InvalidOperationException("Invalid label format");
        }

        return new KeyValuePair<string, string>(split[0], split[1]);
    }
}