using FluentAssertions;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.HelpMenus;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Tests.Assertions;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public class ExecuteTest
{
    [Fact]
    public void Single()
    {
        string[] args = ["abc", "-a"];

        var parser = new ArgumentParser
        {
            Commands =
            [
                new SingleTestCommand(),
            ],
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(2);
        result.ArgumentGroups[0].CommandMatch.Should().BeNull();
        result.ArgumentGroups[0].Arguments.Should().BeEmpty();

        result.ArgumentGroups[1].CommandMatch!.Command.Name.Should().Be("abc");
        result.ArgumentGroups[1].CommandMatch!.Argument.Should().Be("abc");
        result.ArgumentGroups[1].CommandMatch!.ArgumentPosition.Should().Be(1);
        result.ArgumentGroups[1].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[1].Arguments[0].Should().BeOptionWithoutValues("ShortOptA");
    }

    [Fact]
    public void Multiple()
    {
        string[] args = ["abc", "--opt", "1234", "def", "gg"];

        var parser = new ArgumentParser
        {
            Commands =
            [
                new MultipleTestAbcCommand(),
            ],
        };

        var result = parser.Tokenize(args);
        result.ArgumentGroups.Should().HaveCount(3);
        result.ArgumentGroups[0].CommandMatch.Should().BeNull();
        result.ArgumentGroups[0].Arguments.Should().BeEmpty();

        result.ArgumentGroups[1].CommandMatch!.Command.Name.Should().Be("abc");
        result.ArgumentGroups[1].CommandMatch!.Argument.Should().Be("abc");
        result.ArgumentGroups[1].CommandMatch!.ArgumentPosition.Should().Be(1);
        result.ArgumentGroups[1].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[1].Arguments[0].Should().BeOptionWithValue("Opt", "1234");

        result.ArgumentGroups[2].CommandMatch!.Command.Name.Should().Be("def");
        result.ArgumentGroups[2].CommandMatch!.Argument.Should().Be("def");
        result.ArgumentGroups[2].CommandMatch!.ArgumentPosition.Should().Be(4);
        result.ArgumentGroups[2].Arguments.Should().HaveCount(1);
        result.ArgumentGroups[2].Arguments[0].Should().BeValue("gg");
    }

    [Fact]
    public void Execute()
    {
        string[] args = ["abc", "-a"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser
        {
            Commands =
            [
                new ExecuteTestCommand((context, options) =>
                {
                    context.Arguments.Should().BeEquivalentTo(args, o => o.WithStrictOrdering());
                    context.Command.Should().Be("abc");
                    context.ArgumentPosition.Should().Be(1);

                    commandsRun.Add(context.Command!);
                    options.ShortOptA.Should().BeTrue();
                }),
            ],
        };

        var result = parser.Execute(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void Execute_Help()
    {
        string[] args = ["--help"];
        var helpProvided = false;

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            HelpMenuProviderFactory = (parser, commandList) =>
            {
                helpProvided = true;
                commandList.Should().BeEmpty();
                return new HelpMenuProvider(parser, commandList);
            },
        };

        var result = parser.Execute(args);
        helpProvided.Should().BeTrue();
    }

    [Fact]
    public void ExecuteAsync_Help()
    {
        string[] args = ["--help"];
        var helpProvided = false;

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            HelpMenuProviderFactory = (parser, commandList) =>
            {
                helpProvided = true;
                commandList.Should().BeEmpty();
                return new HelpMenuProvider(parser, commandList);
            },
        };

        var result = parser.ExecuteAsync(args);
        helpProvided.Should().BeTrue();
    }

    [Fact]
    public void Execute_Help_Command()
    {
        string[] args = ["abc", "--help"];
        var commandsRun = new List<string>();
        var helpProvided = false;

        var command = new ExecuteHelpCommandTestCommand(context =>
        {
            commandsRun.Add(context.Command!);
        });

        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            Commands = [command],
            HelpMenuProviderFactory = (parser, commandList) =>
            {
                helpProvided = true;
                commandList.Select(c => c.Name).Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
                return new HelpMenuProvider(parser, commandList);
            },
        };

        var result = parser.Execute(args);
        helpProvided.Should().BeTrue();
        commandsRun.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync()
    {
        string[] args = ["abc", "-a"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser
        {
            Commands =
            [
                new ExecuteAsyncTestCommand((context, options) =>
                {
                    commandsRun.Add(context.Command!);
                    options.ShortOptA.Should().BeTrue();
                }),
            ],
        };

        var result = await parser.ExecuteAsync(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void ExecuteAsync_AsSync()
    {
        string[] args = ["abc", "-a"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser
        {
            Commands =
            [
                new ExecuteAsyncTestCommand((context, options) =>
                {
                    commandsRun.Add(context.Command!);
                    options.ShortOptA.Should().BeTrue();
                }),
            ],
        };

        var result = parser.Execute(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public async Task Execute_NoOptions()
    {
        string[] args = ["abc"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser();
        parser.AddCommand("abc", context => commandsRun.Add(context.Command!));

        var result = await parser.ExecuteAsync(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public async Task ExecuteAsync_NoOptions()
    {
        string[] args = ["abc"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser();
        parser.AddCommand("abc", context =>
        {
            commandsRun.Add(context.Command!);
            return Task.CompletedTask;
        });

        var result = await parser.ExecuteAsync(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void ExecuteAsync_NoOptions_AsSync()
    {
        string[] args = ["abc"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser();
        parser.AddCommand("abc", context =>
        {
            commandsRun.Add(context.Command!);
            return Task.CompletedTask;
        });

        var result = parser.Execute(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public async Task ExecuteAsync_Delegate()
    {
        string[] args = ["abc"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser();
        parser.AddCommand<ExecuteTestOptions>("abc", (context, options) =>
        {
            commandsRun.Add(context.Command!);
            return Task.CompletedTask;
        });

        var result = await parser.ExecuteAsync(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void Execute_Delegate()
    {
        string[] args = ["abc"];
        var commandsRun = new List<string>();

        var parser = new ArgumentParser();
        parser.AddCommand<ExecuteTestOptions>("abc", (context, options) =>
        {
            commandsRun.Add(context.Command!);
        });

        var result = parser.Execute(args);
        result.Should().Be(0);
        commandsRun.Should().BeEquivalentTo(["abc"], o => o.WithStrictOrdering());
    }

    [Fact]
    public async Task ExecuteAsync_Error()
    {
        string[] args = ["abc", "-a"];

        var parser = new ArgumentParser();
        parser.AddCommand<ExecuteTestOptions>("abc", (context, options) => { });
        parser.ThrowOnUnknownOptions = true;

        var result = await parser.ExecuteAsync(args);
        result.Should().Be(1);
    }

    [Fact]
    public void Execute_Error()
    {
        string[] args = ["abc", "-a"];

        var parser = new ArgumentParser();
        parser.AddCommand<ExecuteTestOptions>("abc", (context, options) => { });
        parser.ThrowOnUnknownOptions = true;

        var result = parser.Execute(args);
        result.Should().Be(1);
    }

    [Fact]
    public void Execute_UnknownCommand()
    {
        string[] args = ["abc"];
        var errorHandled = false;

        var parser = new ArgumentParser();
        parser.ArgumentParseErrorHandler = e =>
        {
            var unknownCommand = (UnknownCommandException)e;
            unknownCommand.Argument.Should().Be("abc");
            unknownCommand.ArgumentPosition.Should().Be(1);
            errorHandled = true;
        };
        parser.AddCommand<ExecuteTestOptions>("abcd", (context, options) => { });

        var result = parser.Execute(args);
        result.Should().Be(1);

        errorHandled.Should().BeTrue();
    }

    private class SingleTestCommand : Command
    {
        public SingleTestCommand()
        {
            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ];
        }

        public override string Name => "abc";

        public override void Execute(CommandExecutionContext context)
        {
        }
    }

    private class MultipleTestAbcCommand : Command
    {
        public MultipleTestAbcCommand()
        {
            Options =
            [
                Option.LongOptionWithValue("Opt", "opt"),
            ];
            Commands =
            [
                new MultipleTestDefCommand(),
            ];
        }

        public override string Name => "abc";

        public override void Execute(CommandExecutionContext context)
        {
            context.Command.Should().Be(Name);
            context.ArgumentPosition.Should().Be(1);
        }
    }

    private class MultipleTestDefCommand : Command
    {
        public MultipleTestDefCommand()
        {
            Values =
            [
                Value.All("Value"),
            ];
        }

        public override string Name => "def";

        public override void Execute(CommandExecutionContext context)
        {
            context.Command.Should().Be(Name);
            context.ArgumentPosition.Should().Be(4);
        }
    }

    private class ExecuteTestCommand : Command<ExecuteTestOptions>
    {
        private readonly Action<CommandExecutionContext, ExecuteTestOptions> _executeCallback;

        public ExecuteTestCommand(Action<CommandExecutionContext, ExecuteTestOptions> executeCallback)
        {
            _executeCallback = executeCallback;

            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ];
        }

        public override string Name => "abc";

        public override void Execute(CommandExecutionContext context, ExecuteTestOptions options)
        {
            _executeCallback(context, options);
        }
    }

    private class ExecuteAsyncTestCommand : AsyncCommand<ExecuteTestOptions>
    {
        private readonly Action<CommandExecutionContext, ExecuteTestOptions> _executeCallback;

        public ExecuteAsyncTestCommand(Action<CommandExecutionContext, ExecuteTestOptions> executeCallback)
        {
            _executeCallback = executeCallback;

            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ];
        }

        public override string Name => "abc";

        public override Task ExecuteAsync(CommandExecutionContext context, ExecuteTestOptions options)
        {
            _executeCallback(context, options);
            return Task.CompletedTask;
        }
    }

    private class ExecuteTestOptions
    {
        public bool ShortOptA { get; set; }
    }

    private class ExecuteHelpCommandTestCommand : Command
    {
        private readonly Action<CommandExecutionContext> _executeCallback;

        public ExecuteHelpCommandTestCommand(Action<CommandExecutionContext> executeCallback)
        {
            _executeCallback = executeCallback;

            Options =
            [
                Option.ShortOptionSwitch("ShortOptA", 'a'),
            ];
        }

        public override string Name => "abc";

        public override void Execute(CommandExecutionContext context)
        {
            _executeCallback(context);
        }
    }
}
