using FluentAssertions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.HelpMenus.HelpMenuProviderTests;

public class GetUnknownCommandMessageTest
{
    [Fact]
    public void Single()
    {
        GetUnknownCommandMessage(["abcdef"], "abcdee")
            .Should().Be(@"Error: ""abcdee"" is not a known command.

The most similar commands are:
        abcdef
");
    }

    [Fact]
    public void Multiple()
    {
        GetUnknownCommandMessage(["abcdef", "abcd"], "abcdee")
            .Should().Be(@"Error: ""abcdee"" is not a known command.

The most similar commands are:
        abcdef
        abcd
");
    }

    [Fact]
    public void SomeSimilar()
    {
        GetUnknownCommandMessage(["abcdef", "zzz"], "abcdee")
            .Should().Be(@"Error: ""abcdee"" is not a known command.

The most similar commands are:
        abcdef
");
    }

    [Fact]
    public void CaseInsensitive()
    {
        var parser = new ArgumentParser();
        parser.AddCommand("ABCDEF", _ => { }, command =>
        {
            command.CaseInsensitiveNameMatch = true;
        });

        var writer = new StringWriter();
        parser.OutputWriter = writer;

        var result = parser.Execute(["abcdee"]);
        result.Should().Be(1);

        var output = writer.ToString();
        output.Should().Be(@"Error: ""abcdee"" is not a known command.

The most similar commands are:
        ABCDEF
");
    }

    private string GetUnknownCommandMessage(IEnumerable<string> commands, string value)
    {
        var parser = new ArgumentParser();

        foreach (var command in commands)
        {
            parser.AddCommand(command, _ => { });
        }

        var writer = new StringWriter();
        parser.OutputWriter = writer;

        var result = parser.Execute([value]);
        result.Should().Be(1);

        return writer.ToString();
    }
}
