using FluentAssertions;
using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.ExtensionTests;

public class ArgumentRegistrationPropertiesExtensionTests
{
    [Fact]
    public void WithOptionsFrom()
    {
        var parser = new ArgumentParser();
        parser.WithOptionsFrom<Instance>();

        parser.Options.Should().HaveCount(1);
        parser.Options.First().Name.Should().Be("OptionA");
    }

    [Fact]
    public void WithValuesFrom()
    {
        var parser = new ArgumentParser();
        parser.WithValuesFrom<Instance>();

        parser.Values.Should().HaveCount(1);
        parser.Values.First().Name.Should().Be("ValueA");
    }

    [Fact]
    public void WithOptionsValuesFrom()
    {
        var parser = new ArgumentParser();
        parser.WithOptionsValuesFrom<Instance>();

        parser.Options.Should().HaveCount(1);
        parser.Options.First().Name.Should().Be("OptionA");
        parser.Values.Should().HaveCount(1);
        parser.Values.First().Name.Should().Be("ValueA");
    }

    [Fact]
    public void AddCommand()
    {
        var parser = new ArgumentParser();
        parser.AddCommand("test", Nop, cmd =>
        {
            cmd.Description = "test command";
        });

        parser.Commands.Should().HaveCount(1);
        var command = parser.Commands.First();
        command.Name.Should().Be("test");
        command.Description.Should().Be("test command");
    }

    [Fact]
    public void AddCommand_Multiple_Nested()
    {
        var parser = new ArgumentParser();
        parser
            .AddCommand("test", Nop, cmd =>
            {
                cmd.Description = "test command";
                cmd.AddCommand("nested", Nop, cmd =>
                {
                    cmd.Description = "nested command";
                });
            })
            .AddCommand("asdf", Nop, cmd =>
            {
                cmd.Aliases = ["hjkl"];
            });

        parser.Commands.Should().HaveCount(2);
        var command = parser.Commands.ElementAt(0);
        command.Name.Should().Be("test");
        command.Description.Should().Be("test command");
        command.Commands.Should().HaveCount(1);
        var nestedCommand = command.Commands.First();
        nestedCommand.Name.Should().Be("nested");
        nestedCommand.Description.Should().Be("nested command");

        command = parser.Commands.ElementAt(1);
        command.Name.Should().Be("asdf");
        command.Aliases.Should().BeEquivalentTo(["hjkl"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void AddCommand_Options()
    {
        var parser = new ArgumentParser();
        parser.AddCommand<CommandOptions>("test", Nop, cmd =>
        {
            cmd.Description = "test command";
        });

        parser.Commands.Should().HaveCount(1);
        var command = parser.Commands.First();
        command.Name.Should().Be("test");
        command.Description.Should().Be("test command");
    }

    [Fact]
    public void AddCommand_Async()
    {
        var parser = new ArgumentParser();
        parser.AddCommand("test", NopAsync, cmd =>
        {
            cmd.Description = "test command";
        });

        parser.Commands.Should().HaveCount(1);
        var command = parser.Commands.First();
        command.Name.Should().Be("test");
        command.Description.Should().Be("test command");
    }

    [Fact]
    public void AddCommand_AsyncOptions()
    {
        var parser = new ArgumentParser();
        parser.AddCommand<CommandOptions>("test", NopAsync, cmd =>
        {
            cmd.Description = "test command";
        });

        parser.Commands.Should().HaveCount(1);
        var command = parser.Commands.First();
        command.Name.Should().Be("test");
        command.Description.Should().Be("test command");
    }

    private void Nop(CommandExecutionContext context)
    {
    }

    private Task NopAsync(CommandExecutionContext context)
        => Task.CompletedTask;

    private void Nop(CommandExecutionContext context, CommandOptions options)
    {
    }

    private Task NopAsync(CommandExecutionContext context, CommandOptions options)
        => Task.CompletedTask;

    private class Instance
    {
        [Option]
        public string? OptionA { get; set; }

        [Value(0)]
        public string? ValueA { get; set; }
    }

    private class CommandOptions
    {
    }
}
