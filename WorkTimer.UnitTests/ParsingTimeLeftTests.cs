using FluentAssertions;
using WorkTimer.Console;

namespace WorkTimer.UnitTests;

public class ParsingTimeLeftTests
{
    [Fact]
    public void No_units_means_minutes()
    {
        var timeLeft = TimeLeft.FromString("45");

        timeLeft.InSeconds.Should().Be(2700);
        
    }
    
    [Fact]
    public void Only_seconds()
    {
        var timeLeft = TimeLeft.FromString("15s");

        timeLeft.InSeconds.Should().Be(15);
    }

    [Fact]
    public void Only_minutes()
    {
        var timeLeft = TimeLeft.FromString("10m");

        timeLeft.InSeconds.Should().Be(600);
    }

    [Fact]
    public void Only_hours()
    {
        var timeLeft = TimeLeft.FromString("2h");

        timeLeft.InSeconds.Should().Be(7200);
    }

    [Theory]
    [InlineData("x40m")]
    [InlineData("15x100m")]
    [InlineData("aaa")]
    [InlineData("15m10")]
    [InlineData("15m2h1s")]
    public void InvalidNumber(string invalid)
    {
        var action = () =>TimeLeft.FromString(invalid);
        action.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void hours_and_minutes()
    {
        var timeLeft = TimeLeft.FromString("2h30m");

        timeLeft.InSeconds.Should().Be(7200 + 1800);
    }

    [Fact]
    public void Hours_Minutes_seconds()
    {
        var timeLeft = TimeLeft.FromString("2h30m15s");

        timeLeft.InSeconds.Should().Be(7200 + 1800 + 15);
    }
    
    [Fact]
    public void Hours_and_seconds()
    {
        var timeLeft = TimeLeft.FromString("2h15s");

        timeLeft.InSeconds.Should().Be(7200 + 15);
    }
    
    [Fact]
    public void Minutes_and_seconds()
    {
        var timeLeft = TimeLeft.FromString("31m15s");

        timeLeft.InSeconds.Should().Be(1860 + 15);
    }
}