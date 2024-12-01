using FluentAssertions;
using System.ComponentModel;
using System.Globalization;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public class ParseDeserializeAttributeTest : ParserBaseTest
{
    [Fact]
    public void IntConverter()
    {
        string[] args = ["--abc", "123", "--def", "321"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "abc"),
                Option.LongOptionWithValue("ValueB", "def"),
            ],
        };

        var result = parser.ParseTo<IntTestObject>(args);
        result.ValueA.Should().Be(123);
        result.ValueB.Should().Be(421);
    }

    [Fact]
    public void IntConverter_List()
    {
        string[] args = ["--ghi", "321", "123"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("ValueC", "ghi", 1, null),
            ],
        };

        var result = parser.ParseTo<IntTestObject>(args);
        result.ValueC.Should().BeEquivalentTo([421, 223], o => o.WithStrictOrdering());
    }

    [Fact]
    public void DefaultValue()
    {
        string[] args = ["--abc", "123"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "abc"),
                Option.LongOptionWithValue("ValueB", "def"),
            ],
        };

        var result = parser.ParseTo<IntDefaultTestObject>(args);
        result.ValueA.Should().Be(123);
        result.ValueB.Should().Be(6);
    }


    private class IntTestObject
    {
        public int ValueA { get; set; }

        [TypeConverter(typeof(IntTestConverter))]
        public int ValueB { get; set; }

        [TypeConverter(typeof(IntListTestConverter))]
        public List<int>? ValueC { get; set; }
    }

    private class IntDefaultTestObject
    {
        [DefaultValue(4)]
        public int ValueA { get; set; }

        [DefaultValue(6)]
        public int ValueB { get; set; }
    }

    private class IntTestConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string)
            || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                return int.Parse(str) + 100;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    private class IntListTestConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string)
            || sourceType == typeof(IReadOnlyList<string>)
            || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                return int.Parse(str) + 100;
            }
            else if (value is IReadOnlyList<string> strs)
            {
                return strs.Select(x => int.Parse(x) + 100).ToArray();
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
