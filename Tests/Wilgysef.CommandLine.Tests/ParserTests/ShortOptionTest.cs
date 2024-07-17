using FluentAssertions;

namespace Wilgysef.CommandLine.Tests.ParserTests;

public class ShortOptionTest
{
    [Fact]
    public void ShortOption()
    {
        string[] args = ["-a"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOption("ShortOpt", 'a'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOpt");
        result.Arguments[0].Values.Should().BeNull();
    }

    [Fact]
    public void ShortOption_WithValue()
    {
        string[] args = ["-a", "4"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOpt", 'a'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOpt");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("4");
    }

    [Fact]
    public void ShortOption_WithValue_Equals()
    {
        string[] args = ["-a=4"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOpt", 'a'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOpt");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("4");
    }

    [Fact]
    public void ShortOption_WithValue_ImmediateAfter()
    {
        string[] args = ["-a4"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOpt", 'a'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOpt");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("4");
    }

    [Fact]
    public void ShortOption_WithValue_Missing()
    {
        string[] args = ["-a"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOpt", 'a'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOpt");
        result.Arguments[0].Values.Should().BeNull();
    }

    [Fact]
    public void ShortOption_WithValue_Missing_OtherOption()
    {
        string[] args = ["-a", "-b"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionWithValue("ShortOptB", 'b'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(2);
        result.Arguments[0].Name.Should().Be("ShortOptA");
        result.Arguments[0].Values.Should().BeNull();
        result.Arguments[1].Name.Should().Be("ShortOptB");
        result.Arguments[1].Values.Should().BeNull();
    }

    [Fact]
    public void ShortOption_Multiple()
    {
        string[] args = ["-abc"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOption("ShortOptA", 'a'),
                Option.ShortOption("ShortOptB", 'b'),
                Option.ShortOption("ShortOptC", 'c'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(3);
        result.Arguments[0].Name.Should().Be("ShortOptA");
        result.Arguments[0].Values.Should().BeNull();
        result.Arguments[1].Name.Should().Be("ShortOptB");
        result.Arguments[1].Values.Should().BeNull();
        result.Arguments[2].Name.Should().Be("ShortOptC");
        result.Arguments[2].Values.Should().BeNull();
    }

    [Fact]
    public void ShortOption_Multiple_WithValueAtBeginning()
    {
        string[] args = ["-abc"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOption("ShortOptB", 'b'),
                Option.ShortOption("ShortOptC", 'c'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(1);
        result.Arguments[0].Name.Should().Be("ShortOptA");
        result.Arguments[0].Values.Should().HaveCount(1);
        result.Arguments[0].Values[0].Should().Be("bc");
    }

    [Fact]
    public void ShortOption_WithValueAtMiddle()
    {
        string[] args = ["-bac"];

        var parser = new Parser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOption("ShortOptB", 'b'),
                Option.ShortOption("ShortOptC", 'c'),
            ],
        };

        var result = parser.Parse(args);
        result.Arguments.Count.Should().Be(2);
        result.Arguments[0].Name.Should().Be("ShortOptB");
        result.Arguments[0].Values.Should().BeNull();
        result.Arguments[1].Name.Should().Be("ShortOptA");
        result.Arguments[1].Values.Should().HaveCount(1);
        result.Arguments[1].Values[0].Should().Be("c");
    }
}
