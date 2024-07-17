using FluentAssertions;

namespace Wilgysef.CommandLine.Tests.ParserTests;

public class LongOptionTest
{
    [Fact]
    public void LongOption()
    {
        string[] args = ["--a"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOption("LongOptA", "a"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().BeNull();
    }

    [Fact]
    public void LongOption_WithValue()
    {
        string[] args = ["--a", "4"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("4");
    }

    [Fact]
    public void LongOption_WithValue_Equals()
    {
        string[] args = ["--a=4"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("4");
    }

    [Fact]
    public void LongOption_WithValue_Missing()
    {
        string[] args = ["--a"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().BeNull();
    }

    [Fact]
    public void LongOption_WithValue_Missing_OtherOption()
    {
        string[] args = ["--a", "--b"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
                Option.LongOptionWithValue("LongOptB", "b"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(2);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().BeNull();
        result.Arguments[1].Name.Should().Be("LongOptB");
        result.Arguments[1].Values.Should().BeNull();
    }

    [Fact]
    public void LongOption_Literal()
    {
        string[] args = ["--a", "--", "--b"];

        var parser = new Parser
        {
            Options =
            [
                Option.LongOption("LongOptA", "a"),
                Option.LongOption("LongOptB", "b"),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(2);
        result.Arguments[0].Name.Should().Be("LongOptA");
        result.Arguments[0].Values.Should().BeNull();
        result.Arguments[1].Name.Should().BeNull();
        result.Arguments[1].Values.Should().HaveCount(1);
        result.Arguments[1].Values![0].Should().Be("--b");
    }
}
