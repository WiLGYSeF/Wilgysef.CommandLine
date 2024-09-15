using FluentAssertions;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.HelpMenus;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.HelpMenus;

public class HelpMenuProviderTest
{
    [Fact]
    public void LongOption()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Value", "test"),
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test      
"));
    }

    [Fact]
    public void LongOption_WithValue()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ValueCountRange = new ValueRange(1, 1),
                    ValueNames = ["input"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test <input>      Test option
"));
    }

    [Fact]
    public void LongOption_ValueRange()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ValueCountRange = new ValueRange(1, 3),
                    ValueNames = ["input", "output", "backup"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test <input> [output] [backup]      Test option
"));
    }

    [Fact]
    public void LongOption_ValueRange_OneName()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ValueCountRange = new ValueRange(1, 3),
                    ValueNames = ["file"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test <file> [file] [file]      Test option
"));
    }

    [Fact]
    public void LongOption_ValueRange_Infinite()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ValueCountRange = new ValueRange(1, null),
                    ValueNames = ["file"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test <file> ...      Test option
"));
    }

    [Fact]
    public void LongOption_SwitchNegateLongPrefix()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    Switch = true,
                    SwitchNegateLongPrefix = "no-",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --[no-]test      Test option
"));
    }

    [Fact]
    public void Description()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ValueCountRange = new ValueRange(1, 1),
                    ValueNames = ["input"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        helpProvider.Description = "Description of app";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Description:
    Description of app

Usage:
    TestApp [options]

Options:
    --test <input>      Test option
"));
    }

    [Fact]
    public void LongAndShortOption_WithValue()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                    ShortNames = ['t'],
                    ValueCountRange = new ValueRange(1, 1),
                    ValueNames = ["input"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    -t, --test <input>      Test option
"));
    }

    [Fact]
    public void Value()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
            ],
            Values =
            [
                new Value("Path", new ValueRange(0))
                {
                    Description = "Test value",
                    ValueName = "path",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp <path> [options]

Values:
    path      Test value

Options:
    --test      Test option
"));
    }

    [Fact]
    public void Value_NoValueName()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
            ],
            Values = [
                new Value("Path", new ValueRange(0))
                {
                    Description = "Test value",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp <Path> [options]

Values:
    Path      Test value

Options:
    --test      Test option
"));
    }

    [Fact]
    public void Usage_ValueGap()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
            ],
            Values =
            [
                new Value("Path", new ValueRange(1))
                {
                    Description = "Test value",
                    ValueName = "path",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp <value> <path> [options]

Values:
    path      Test value

Options:
    --test      Test option
"));
    }

    [Fact]
    public void Usage_ValueInfinite()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
            ],
            Values =
            [
                new Value("Path", new ValueRange(0, null))
                {
                    Description = "Test value",
                    ValueName = "path",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp <path> ... [options]

Values:
    path      Test value

Options:
    --test      Test option
"));
    }

    [Fact]
    public void Usage_ValueMultiple()
    {
        var parser = new ArgumentParser
        {
            Values =
            [
                new Value("Path", new ValueRange(0, 2))
                {
                    Description = "Test value",
                    ValueName = "path",
                },
                new Value("OtherVal", new ValueRange(5))
                {
                    Description = "Other value",
                    ValueName = "other",
                },
                new Value("Abc", new ValueRange(4))
                {
                    Description = "Abc value",
                    ValueName = "abc",
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp <path> <path> <path> <value> <abc> <other>

Values:
    path       Test value
    abc        Abc value
    other      Other value
"));
    }

    [Fact]
    public void Command()
    {
        var command = new CommandTestCommand();

        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
            ],
            Commands = [command],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options] [command]

Commands:
    abc      Test command

Options:
    --test      Test option
"));

        helpProvider.SetCommandList([command]);
        result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp abc [options]

Options:
    --other      Other option
"));
    }

    [Fact]
    public void Option_LongDescription()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "Test option",
                    LongNames = ["test"],
                },
                new Option("Test2")
                {
                    Description = "This is a long option description that causes the table to wrap",
                    LongNames = ["test2"],
                },
                new Option("Test3")
                {
                    Description = "Test option 3",
                    LongNames = ["test3"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        helpProvider.MaxRowLength = 50;

        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test       Test option
    --test2
                 This is a long option description
                 that causes the table to wrap
    --test3      Test option 3
"));
    }

    [Fact]
    public void Option_LongDescription_Only()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    Description = "This is a long option description that causes the table to wrap",
                    LongNames = ["test"],
                },
                new Option("Test2")
                {
                    Description = "This is another long option description that causes the table to wrap",
                    LongNames = ["test2"],
                },
            ],
        };

        var helpProvider = new HelpMenuProvider(parser);
        helpProvider.ApplicationName = "TestApp";
        helpProvider.MaxRowLength = 50;

        var result = string.Join("\n", helpProvider.GetHelpMenu());
        result.Should().Be(Prep(
@"Usage:
    TestApp [options]

Options:
    --test
        This is a long option description that
        causes the table to wrap
    --test2
        This is another long option description
        that causes the table to wrap
"));
    }

    private static string Prep(string s)
    {
        return s.Replace("\r\n", "\n");
    }

    private class CommandTestCommand : Command
    {
        public CommandTestCommand()
        {
            Description = "Test command";
            Options =
            [
                new Option("Test")
                {
                    LongNames = ["other"],
                    Description = "Other option",
                },
            ];
        }

        public override string Name => "abc";

        public override void Execute(CommandExecutionContext context)
        {
        }
    }
}
