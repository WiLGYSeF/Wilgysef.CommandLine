using FluentAssertions;
using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Values;

namespace Wilgysef.CommandLine.Tests.Documentation;

public class DocumentationTest
{
    [Fact]
    public void OptionConfigClassSimple()
    {
        string[] args = ["--name", "John"];

        var parser = new ArgumentParser();
        parser.WithOptionsValuesFrom<OptionConfigClassSimpleTest>();

        var options = parser.ParseTo<OptionConfigClassSimpleTest>(args);

        options.Name.Should().Be("John");
    }

    [Fact]
    public void OptionCodeClassSimple()
    {
        string[] args = ["--name", "John"];

        var parser = new ArgumentParser();

        var inputOption = Option.LongOptionWithValue("Name", "name");
        inputOption.Description = "Name to use";
        parser.Options.Add(inputOption);

        var options = parser.ParseTo<OptionCodeClassSimpleTest>(args);

        options.Name.Should().Be("John");
    }

    [Fact]
    public void OptionCodeClassSimple_Alt()
    {
        string[] args = ["--name", "John"];

        var parser = new ArgumentParser();
        parser.Options.Add(new Option("Name")
        {
            LongNames = ["name"],
            Description = "Name to use",
            ValueCountRange = ValueRange.Exactly(1),
        });

        var options = parser.ParseTo<OptionCodeClassSimpleTest>(args);

        options.Name.Should().Be("John");
    }

    [Fact]
    public void ValueConfigClassSimple()
    {
        string[] args = ["--name", "John", "output.txt", "3", "extra", "args"];

        var parser = new ArgumentParser();
        parser.WithOptionsValuesFrom<ValueConfigClassSimpleTest>();

        var options = parser.ParseTo<ValueConfigClassSimpleTest>(args);

        options.Name.Should().Be("John");
        options.File.Should().Be("output.txt");
        options.Number.Should().Be(3);
        options.Extra.Should().BeEquivalentTo(["extra", "args"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void ValueCodeClassSimple()
    {
        string[] args = ["--name", "John", "output.txt", "3", "extra", "args"];

        var parser = new ArgumentParser();

        parser.Options.Add(new Option("Name")
        {
            LongNames = ["name"],
            Description = "Name to use",
            ValueCountRange = ValueRange.Exactly(1),
        });

        var file = Value.Single("File", 0);
        file.Description = "Output file";
        file.ValueName = "FILE";
        parser.Values.Add(file);

        var number = Value.Single("Number", 1);
        number.Description = "Number";
        number.ValueName = "NUM";
        parser.Values.Add(number);

        parser.Values.Add(new Value("Extra", 2, null)
        {
            Description = "Extra values",
        });

        var options = parser.ParseTo<ValueConfigClassSimpleTest>(args);

        options.Name.Should().Be("John");
        options.File.Should().Be("output.txt");
        options.Number.Should().Be(3);
        options.Extra.Should().BeEquivalentTo(["extra", "args"], o => o.WithStrictOrdering());
    }

    private class OptionConfigClassSimpleTest
    {
        [Option(Description = "Name to use")]
        [LongName("name")]
        public string? Name { get; set; }
    }

    private class OptionCodeClassSimpleTest
    {
        public string? Name { get; set; }
    }

    private class ValueConfigClassSimpleTest
    {
        [Option(Description = "Name to use")]
        [LongName("name")]
        public string? Name { get; set; }

        [Value(0, Description = "Output file", ValueName = "FILE")]
        public string? File { get; set; }

        [Value(1, Description = "Number", ValueName = "NUM")]
        public int? Number { get; set; }

        [Value(2, int.MaxValue, Description = "Extra values")]
        public string[]? Extra { get; set; }
    }
}
