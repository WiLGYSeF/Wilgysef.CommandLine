using FluentAssertions;
using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.Parsers;

public class ValueBuilderTest
{
    [Fact]
    public void Value()
    {
        var values = ValueBuilder.BuildValues<ValueTest>();
        values.Should().HaveCount(2);
        values[0].Name.Should().Be("Values");
        values[0].Description.Should().Be("Values description");
        values[0].ValueName.Should().Be("value");
        values[0].StartIndex.Should().Be(1);
        values[0].EndIndex.Should().Be(3);

        values[1].Name.Should().Be("OtherValue");
        values[1].StartIndex.Should().Be(4);
        values[1].EndIndex.Should().Be(4);
    }

    private class ValueTest
    {
        [Value(
            1,
            3,
            Description = "Values description",
            ValueName = "value")]
        public IReadOnlyList<string>? Values { get; set; }

        [Value(4)]
        public string? OtherValue { get; set; }

        public string? RegularProperty { get; set; }
    }
}
