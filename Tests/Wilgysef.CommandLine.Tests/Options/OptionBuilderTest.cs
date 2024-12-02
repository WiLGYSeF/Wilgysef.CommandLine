using FluentAssertions;
using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Options;

namespace Wilgysef.CommandLine.Tests.Options;

public class OptionBuilderTest
{
    [Fact]
    public void Option()
    {
        var options = OptionBuilder.BuildOptions<OptionTest>();
        options.Should().HaveCount(3);
        options[0].Name.Should().Be("ValueA");
        options[0].Description.Should().Be("Option description");
        options[0].Hidden.Should().BeTrue();
        options[0].ValueNames.Should().BeEquivalentTo(["a", "b", "c"], o => o.WithStrictOrdering());
        options[0].ShortNames.Should().BeNull();
        options[0].LongNames.Should().BeEquivalentTo(["abc", "asdf"], o => o.WithStrictOrdering());
        options[0].ShortNamePrefix.Should().BeNull();
        options[0].LongNamePrefix.Should().Be("--");
        options[0].KeyValueSeparators.Should().BeEquivalentTo(["="], o => o.WithStrictOrdering());
        options[0].ValueCountRange!.Min.Should().Be(0);
        options[0].ValueCountRange!.Max.Should().Be(3);
        options[0].Switch.Should().BeFalse();
        options[0].SwitchNegateLongPrefix.Should().BeNull();
        options[0].SwitchNegateShortNames.Should().BeNull();
        options[0].Counter.Should().BeFalse();
        options[0].Required.Should().BeTrue();
        options[0].GroupNames.Should().BeEquivalentTo(["Group"], o => o.WithStrictOrdering());
        options[0].Unique.Should().Be(true);
        options[0].ShortNameImmediateValue.Should().BeNull();
        options[0].LongNameCaseInsensitive.Should().BeTrue();
        options[0].KeepFirstValue.Should().BeTrue();

        options[1].ShortNames.Should().BeEquivalentTo(['c'], o => o.WithStrictOrdering());
        options[1].LongNames.Should().BeEquivalentTo(["check"], o => o.WithStrictOrdering());
        options[1].ShortNamePrefix.Should().Be("-");
        options[1].Switch.Should().BeTrue();
        options[1].SwitchNegateLongPrefix.Should().Be("no-");
        options[1].SwitchNegateShortNames.Should().BeEquivalentTo(['C'], o => o.WithStrictOrdering());

        options[2].Counter.Should().BeTrue();
        options[2].ShortNames.Should().BeEquivalentTo(['u'], o => o.WithStrictOrdering());
        options[2].LongNames.Should().BeEquivalentTo(["counter"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void ValueCount()
    {
        var options = OptionBuilder.BuildOptions<ValueCountTest>();
        options.Should().HaveCount(5);
        OptionShouldBe(options[0], "ValueA", 2, 2, null, ["value1", "value2"]);
        OptionShouldBe(options[1], "ValueB", 2, 2, null, ["value"]);
        OptionShouldBe(options[2], "ValueC", 1, 2, null, ["value"]);
        OptionShouldBe(options[3], "ValueD", 2, 2, true, ["value"]);
        OptionShouldBe(options[4], "ValueE", 1, 2, true, ["value"]);

        static void OptionShouldBe(Option option, string name, int min, int? max, bool? keepFirstValue, params string[] valueNames)
        {
            option.Name.Should().Be(name);
            option.ValueCountRange!.Min.Should().Be(min);
            option.ValueCountRange!.Max.Should().Be(max);
            option.KeepFirstValue.Should().Be(keepFirstValue);
            option.ValueNames.Should().BeEquivalentTo(valueNames, o => o.WithStrictOrdering());
        }
    }

    private class OptionTest
    {
        [Option(
            Description = "Option description",
            Hidden = true,
            Required = true,
            Unique = true)]
        [Group("Group")]
        [LongName(true, "abc", "asdf", LongNamePrefix = "--")]
        [ValueCount(0, 3, true, "a", "b", "c", KeyValueSeparator = "=")]
        public IReadOnlyList<string>? ValueA { get; set; }

        [Option]
        [ShortName(true, 'c', ShortNamePrefix = "-")]
        [LongName("check")]
        [Switch(SwitchNegateLongPrefix = "no-", SwitchNegateShortName = 'C')]
        public bool ValueB { get; set; }

        [Option]
        [Counter]
        [ShortName('u')]
        [LongName("counter")]
        public int ValueC { get; set; }

        public string? RegularProperty { get; set; }
    }

    private class ValueCountTest
    {
        [Option]
        [LongName("abc")]
        [ValueCount("value1", "value2")]
        public IReadOnlyList<string>? ValueA { get; set; }

        [Option]
        [LongName("abc")]
        [ValueCount(2, "value")]
        public IReadOnlyList<string>? ValueB { get; set; }

        [Option]
        [LongName("abc")]
        [ValueCount(1, 2, "value")]
        public IReadOnlyList<string>? ValueC { get; set; }

        [Option]
        [LongName("abc")]
        [ValueCount(2, true, "value")]
        public IReadOnlyList<string>? ValueD { get; set; }

        [Option]
        [LongName("abc")]
        [ValueCount(1, 2, true, "value")]
        public IReadOnlyList<string>? ValueE { get; set; }
    }
}
