using FluentAssertions;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Tests.Assertions;

namespace Wilgysef.CommandLine.Tests.Parsers.TokenizerTests;

public class LongOptionTest
{
    [Fact]
    public void Base()
    {
        string[] args = ["--a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("LongOptA", "a"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithoutValues("LongOptA");
    }

    [Fact]
    public void Value()
    {
        string[] args = ["--a", "4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("LongOptA", "4");
    }

    [Fact]
    public void Value_Equals()
    {
        string[] args = ["--a=4"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithValue("LongOptA", "4");
        group.Arguments[0].ArgumentIsKeyValue.Should().BeTrue();
    }

    [Fact]
    public void Value_Missing()
    {
        string[] args = ["--a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }

    [Fact]
    public void OptionalValue_Missing()
    {
        string[] args = ["--a"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithOptionalValue("LongOptA", "a"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(1);
        group.Arguments[0].Should().BeOptionWithoutValues("LongOptA");
    }

    [Fact]
    public void Value_Missing_OtherOption()
    {
        string[] args = ["--a", "--b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
                Option.LongOptionWithValue("LongOptB", "b"),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 1
                && e.ExpectedMaxValues == 1
                && e.ActualValues == 0);
    }

    [Fact]
    public void OptionalValue_Missing_OtherOption()
    {
        string[] args = ["--a", "--b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithOptionalValue("LongOptA", "a"),
                Option.LongOptionSwitch("LongOptB", "b"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(2);
        group.Arguments[0].Should().BeOptionWithoutValues("LongOptA");
        group.Arguments[1].Should().BeOptionWithoutValues("LongOptB");
    }

    [Fact]
    public void Literal()
    {
        string[] args = ["--a", "--", "--b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("LongOptA", "a"),
                Option.LongOptionSwitch("LongOptB", "b"),
            ],
        };

        var result = parser.Tokenize(args);
        var group = result.ArgumentGroups[0];
        group.Arguments.Count.Should().Be(2);
        group.Arguments[0].Should().BeOptionWithoutValues("LongOptA");
        group.Arguments[1].Should().BeValue("--b");
    }

    [Fact]
    public void Literal_InValue()
    {
        string[] args = ["--a", "1", "--", "2"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValues("LongOptA", "a", 2),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 2
                && e.ExpectedMaxValues == 2
                && e.ActualValues == 1);
    }

    [Fact]
    public void Command_InValue()
    {
        string[] args = ["--a", "1", "b", "2"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValues("LongOptA", "a", 2),
            ],
        };
        parser.AddCommand("b", context => { });

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 2
                && e.ExpectedMaxValues == 2
                && e.ActualValues == 1);
    }

    [Fact]
    public void Unknown_InValue()
    {
        string[] args = ["--a", "1", "-b"];

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValues("LongOptA", "a", 2),
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--a"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 2
                && e.ExpectedMaxValues == 2
                && e.ActualValues == 1);
    }

    [Fact]
    public void Unknown()
    {
        string[] args = ["--a"];
        var parser = new ArgumentParser();

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<UnknownOptionException>()
            .Where(e => e.Argument == "--a" && e.ArgumentPosition == 1);
    }

    [Fact]
    public void Unknown_Ignore()
    {
        string[] args = ["--a"];
        var parser = new ArgumentParser();
        parser.ThrowOnUnknownOptions = false;

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(0);
    }

    [Fact]
    public void Unknown_NoIgnore()
    {
        string[] args = ["--a"];
        var parser = new ArgumentParser();
        parser.ThrowOnUnknownOptions = false;
        parser.IgnoreUnknownOptions = false;

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeValue("--a");
    }

    [Fact]
    public void UnexpectedKeyValue()
    {
        string[] args = ["--a=4"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("LongOptA", "a"),
            ],
            ThrowOnArgumentUnexpectedKeyValue = true,
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionUnexpectedKeyValueException>()
            .Where(e => e.Argument == "--a=4" && e.ArgumentPosition == 1);
    }

    [Fact]
    public void UnexpectedKeyValue_Ok()
    {
        string[] args = ["--a=4"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("LongOptA", "a"),
            ],
            ThrowOnArgumentUnexpectedKeyValue = false,
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeOptionWithoutValues("LongOptA");
    }

    [Fact]
    public void KeyValue_NoKey()
    {
        string[] args = ["--="];
        var parser = new ArgumentParser();

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<UnknownOptionException>()
            .Where(e => e.Argument == "--=" && e.ArgumentPosition == 1);
    }

    [Fact]
    public void KeyValue_NoValue()
    {
        string[] args = ["--a="];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("LongOptA", "a"),
            ],
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[0].Arguments[0].Should().BeOptionWithValues("LongOptA", [""]);
    }
}
