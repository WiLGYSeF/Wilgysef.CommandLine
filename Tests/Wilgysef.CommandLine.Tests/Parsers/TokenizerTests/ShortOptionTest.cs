using FluentAssertions;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Tests.Assertions;

namespace Wilgysef.CommandLine.Tests.Parsers.TokenizerTests;

public class ShortOptionTest
{
    [Fact]
    public void Base()
    {
        string[] args = ["-a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void Value()
    {
        string[] args = ["-a", "4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("ShortOptA", "4");
    }

    [Fact]
    public void Value_Equals()
    {
        string[] args = ["-a=4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("ShortOptA", "4");
    }

    [Fact]
    public void ValueRange()
    {
        string[] args = ["-a", "4", "6"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValues("ShortOptA", 'a', 2),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValues("ShortOptA", ["4", "6"]);
    }

    [Fact]
    public void Value_ImmediateAfter()
    {
        string[] args = ["-a4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("ShortOptA", "4");
    }

    [Fact]
    public void ValueRange_ImmediateAfter()
    {
        string[] args = ["-a4", "6"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValueRange("ShortOptA", 'a', 2, 2),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValues("ShortOptA", ["4", "6"]);
    }

    [Fact]
    public void Value_Missing()
    {
        string[] args = ["-a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().Throw<ArgumentValuesOutOfRangeException>()
            .Where(e => e.Argument == "-a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }

    [Fact]
    public void OptionalValue_Missing()
    {
        string[] args = ["-a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithOptionalValue("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void Value_Missing_OtherOption()
    {
        string[] args = ["-a", "-b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionWithValue("ShortOptB", 'b'),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().Throw<ArgumentValuesOutOfRangeException>()
            .Where(e => e.Argument == "-a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }

    [Fact]
    public void OptionalValue_Missing_OtherOption()
    {
        string[] args = ["-a", "-b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithOptionalValue("ShortOptA", 'a'),
                Option.ShortOptionWithOptionalValue("ShortOptB", 'b'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(2);
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
        group.Arguments[1].Should().BeOptionWithoutValues("ShortOptB");
    }

    [Fact]
    public void Multiple()
    {
        string[] args = ["-abc"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
                Option.ShortOptionSwitch("ShortOptB", 'b'),
                Option.ShortOptionSwitch("ShortOptC", 'c'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(3);
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
        group.Arguments[1].Should().BeOptionWithoutValues("ShortOptB");
        group.Arguments[2].Should().BeOptionWithoutValues("ShortOptC");
    }

    [Fact]
    public void Multiple_WithValueAtBeginning()
    {
        string[] args = ["-abc"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionSwitch("ShortOptB", 'b'),
                Option.ShortOptionSwitch("ShortOptC", 'c'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("ShortOptA", "bc");
    }

    [Fact]
    public void ValueAtMiddle()
    {
        string[] args = ["-bac"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionSwitch("ShortOptB", 'b'),
                Option.ShortOptionSwitch("ShortOptC", 'c'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(2);
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptB");
        group.Arguments[1].Should().BeOptionWithValue("ShortOptA", "c");
    }

    [Fact]
    public void LongerPrefixNoMatch()
    {
        string[] args = ["-bb"];

        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("DashBA")
                {
                    ShortNamePrefix = "-b",
                    ShortNames = ['a'],
                },
                Option.ShortOptionSwitch("ShortOptB", 'b'),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().Throw<UnknownShortOptionException>()
            .Where(e => e.Argument == "-bb"
                && e.ArgumentPosition == 1
                && e.ShortName == 'b');
    }

    [Fact]
    public void Counter()
    {
        string[] args = ["-a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionCounter("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void Unknown()
    {
        string[] args = ["-a"];

        var parser = new ArgumentParser();
        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<UnknownShortOptionException>()
            .Where(e => e.Argument == "-a" && e.ArgumentPosition == 1 && e.ShortName == 'a');
    }

    [Fact]
    public void Unknown_Part()
    {
        string[] args = ["-ab"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionCounter("ShortOptA", 'a'),
            ],
            ThrowOnUnknownShortOptions = true,
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<UnknownShortOptionException>()
            .Where(e => e.Argument == "-ab" && e.ArgumentPosition == 1 && e.ShortName == 'b');
    }

    [Fact]
    public void Unknown_Part_Ok()
    {
        string[] args = ["-ab"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionCounter("ShortOptA", 'a'),
            ],
            ThrowOnUnknownShortOptions = false,
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void UnexpectedKeyValue()
    {
        string[] args = ["-a=4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ],
            ThrowOnArgumentUnexpectedKeyValue = true,
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<ShortOptionUnexpectedKeyValueException>()
            .Where(e => e.Argument == "-a=4" && e.ArgumentPosition == 1 && e.ShortName == 'a');
    }

    [Fact]
    public void UnexpectedKeyValue_Ok()
    {
        string[] args = ["-a=4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ],
            ThrowOnArgumentUnexpectedKeyValue = false,
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void Value_ImmediateAfter_Throw()
    {
        string[] args = ["-a4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
            ShortNameImmediateValue = false,
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<ShortOptionUnexpectedKeyValueException>()
            .Where(e => e.Argument == "-a4" && e.ArgumentPosition == 1 && e.ShortName == 'a');
    }

    [Fact]
    public void KeyValue_NoKey()
    {
        string[] args = ["-="];

        var parser = new ArgumentParser();
        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<UnknownShortOptionException>()
            .Where(e => e.Argument == "-=" && e.ArgumentPosition == 1 && e.ShortName == '=');
    }

    [Fact]
    public void KeyValue_NoValue()
    {
        string[] args = ["-a="];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
            ],
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeOptionWithValues("ShortOptA", [""]);
    }

    [Fact]
    public void ValueRange_OutOfRange()
    {
        string[] args = ["-a", "-cb"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionWithValue("ShortOptB", 'b'),
                Option.ShortOptionWithValue("ShortOptC", 'c'),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().Throw<ArgumentValuesOutOfRangeException>()
            .Where(e => e.Argument == "-a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }

    [Fact]
    public void ValueRange_OutOfRange_WithFutureSeparator()
    {
        string[] args = ["-a", "-bc=1"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionWithValue("ShortOptA", 'a'),
                Option.ShortOptionWithValue("ShortOptB", 'b'),
                Option.ShortOptionWithValue("ShortOptC", 'c'),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().Throw<ArgumentValuesOutOfRangeException>()
            .Where(e => e.Argument == "-a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }
}
