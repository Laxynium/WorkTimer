using FluentAssertions;
using WorkTimer.Console;

namespace WorkTimer.UnitTests;

public class ParsingCommandLineArgumentsTests
{
    [Fact]
    public void Empty_args()
    {
        var result = ParsedInput.Parse(Array.Empty<string>());
        result.TimeLeft.Should().Be(TimeLeft.FromSeconds(5400));
        result.Labels.Should().BeEmpty();
    }

    [Fact]
    public void Single_arg()
    {
        var result = ParsedInput.Parse(new[] { "46m" });
        result.TimeLeft.Should().Be(TimeLeft.FromSeconds(46 * 60));
        result.Labels.Should().BeEmpty();
    }

    [Fact]
    public void Many_args()
    {
        var result = ParsedInput.Parse(new[] { "1h10m", "id=#1", "name=Important job" });
        result.TimeLeft.Should().Be(TimeLeft.FromSeconds(4200));
        result.Labels.Should().HaveCount(2);
        result.Labels.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "id", "#1" },
            { "name", "Important job" },
        });
    }
}