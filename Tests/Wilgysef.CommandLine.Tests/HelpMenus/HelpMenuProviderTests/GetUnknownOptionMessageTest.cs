using FluentAssertions;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.HelpMenus.HelpMenuProviderTests;

public class GetUnknownOptionMessageTest
{
    [Fact]
    public void LongOption()
    {
        GetUnknownOptionMessage(
            [
                Option.LongOptionSwitch("Opt1", "test"),
            ],
            "--tset")
            .Should().Be(@"Error: ""--tset"" is not a known option.

The most similar options are:
        --test
");
    }

    [Fact]
    public void Multiple_LongOption()
    {
        GetUnknownOptionMessage(
            [
                Option.LongOptionSwitch("Opt1", "test"),
                Option.LongOptionSwitch("Opt2", "tester"),
            ],
            "--tset")
            .Should().Be(@"Error: ""--tset"" is not a known option.

The most similar options are:
        --test
        --tester
");
    }

    [Fact]
    public void SomeSimilar_LongOption()
    {
        GetUnknownOptionMessage(
            [
                Option.LongOptionSwitch("Opt1", "test"),
                Option.LongOptionSwitch("Opt2", "zzz"),
            ],
            "--tset")
            .Should().Be(@"Error: ""--tset"" is not a known option.

The most similar options are:
        --test
");
    }

    [Fact]
    public void CaseInsensitive_LongOption()
    {
        GetUnknownOptionMessage(
            [
                new Option("Opt1")
                {
                    LongNames = ["TEST"],
                    LongNameCaseInsensitive = true,
                },
            ],
            "--tset")
            .Should().Be(@"Error: ""--tset"" is not a known option.

The most similar options are:
        --TEST
");
    }

    [Fact]
    public void Prefix_LongOption()
    {
        GetUnknownOptionMessage(
            [
                new Option("Opt1")
                {
                    LongNamePrefix = "/",
                    LongNames = ["test"],
                },
            ],
            "--tset")
            .Should().Be(@"Error: ""--tset"" is not a known option.

The most similar options are:
        /test
");
    }

    [Fact]
    public void KeyValueSeparator_LongOption()
    {
        GetUnknownOptionMessage(
            [
                new Option("Opt1")
                {
                    LongNames = ["test"],
                    KeyValueSeparators = ["="],
                },
            ],
            "--tset=abc")
            .Should().Be(@"Error: ""--tset=abc"" is not a known option.

The most similar options are:
        --test
");
    }

    [Fact]
    public void SwitchNegation_LongOption()
    {
        GetUnknownOptionMessage(
            [
                new Option("Opt1")
                {
                    LongNames = ["test"],
                    Switch = true,
                    SwitchNegateLongPrefix = "no-",
                },
            ],
            "--no-tset")
            .Should().Be(@"Error: ""--no-tset"" is not a known option.

The most similar options are:
        --test
");
    }

    private string GetUnknownOptionMessage(IEnumerable<Option> options, string value)
    {
        var parser = new ArgumentParser();

        foreach (var option in options)
        {
            parser.Options.Add(option);
        }

        var writer = new StringWriter();
        parser.OutputWriter = writer;

        var result = parser.Execute([value]);
        result.Should().Be(1);

        return writer.ToString();
    }
}
