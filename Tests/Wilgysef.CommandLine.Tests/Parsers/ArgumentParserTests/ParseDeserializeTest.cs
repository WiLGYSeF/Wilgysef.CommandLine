using FluentAssertions;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Values;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public class ParseDeserializeTest : ParserBaseTest
{
    [Fact]
    public void Int()
    {
        string[] args = ["--abc", "4"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "abc"),
            ],
        };

        var result = parser.ParseTo<IntTest>(args);
        result.ValueA.Should().Be(4);
    }

    [Fact]
    public void Int_NoMatch()
    {
        string[] args = [];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueB", "abc"),
            ],
        };

        var result = parser.ParseTo<IntTest>(args);
        result.ValueB.Should().BeNull();
    }

    [Fact]
    public void IntString()
    {
        string[] args = ["--abc", "4", "--str", "asdf"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "abc"),
                Option.LongOptionWithValue("ValueB", "str"),
            ],
        };

        var result = parser.ParseTo<IntStringTest>(args);
        result.ValueA.Should().Be(4);
        result.ValueB.Should().Be("asdf");
    }

    [Fact]
    public void MultipleArgs_Single()
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "str"),
            ],
        };

        var result = parser.ParseTo<StringTest>(args);
        result.ValueA.Should().Be("1234");
    }

    [Fact]
    public void MultipleArgs_Single_First()
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("ValueA", "str"),
            ],
            KeepFirstValue = true,
        };

        var result = parser.ParseTo<StringTest>(args);
        result.ValueA.Should().Be("asdf");
    }

    [Fact]
    public void String_Constructor()
    {
        string[] args = ["--str", "asdf"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
            KeepFirstValue = true,
        };

        var result = parser.ParseTo<StringConstructorTest>(args);
        result.Value!.Value.Should().Be("asdf");
    }

    [Fact]
    public void StringEnumerable_Constructor()
    {
        string[] args = ["--str", "asdf", "test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Value")
                {
                    LongNames = ["str"],
                    ValueCountRange = new ValueRange(2, 2),
                },
            ],
            KeepFirstValue = true,
        };

        var result = parser.ParseTo<StringEnumerableConstructorTest>(args);
        result.Value!.Values.Should().BeEquivalentTo(["asdf", "test"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void Switch()
    {
        string[] args = ["--abc", "--def"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("ValueA", "abc"),
                Option.LongOptionSwitch("ValueB", "def"),
            ],
        };

        var result = parser.ParseTo<BoolTest>(args);
        result.ValueA.Should().BeTrue();
        result.ValueB.Should().BeTrue();
    }

    [Fact]
    public void Switch_Negate()
    {
        string[] args = ["--abc", "--no-abc"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionNegatableSwitch("ValueA", "abc"),
            ],
        };

        var result = parser.ParseTo<BoolTest>(args);
        result.ValueA.Should().BeFalse();

        args = ["-a", "-A"];
        parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionNegatableSwitch("ValueA", 'a', 'A'),
            ],
        };

        result = parser.ParseTo<BoolTest>(args);
        result.ValueA.Should().BeFalse();
    }

    [Fact]
    public void Counter()
    {
        string[] args = ["--abc", "--def", "--abc"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionCounter("ValueA", "abc"),
                Option.LongOptionCounter("ValueB", "def"),
            ],
        };

        var result = parser.ParseTo<IntTest>(args);
        result.ValueA.Should().Be(2);
        result.ValueB.Should().Be(1);
    }

    [Fact]
    public void ParserValue()
    {
        string[] args = ["abc", "def"];
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("ValueA", 0),
                Value.Single("ValueB", 1),
            ],
        };

        var result = parser.ParseTo<StringTest>(args);
        result.ValueA.Should().Be("abc");
        result.ValueB.Should().Be("def");
    }

    [Fact]
    public void Value_SkippedFirst()
    {
        string[] args = ["abc", "def"];
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("ValueB", 1),
            ],
        };

        var result = parser.ParseTo<StringTest>(args);
        result.ValueA.Should().BeNull();
        result.ValueB.Should().Be("def");
    }

    [Fact]
    public void Value_MissingProperty()
    {
        string[] args = ["abc", "def"];
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("ZZZ", 0),
            ],
            ThrowOnMissingProperty = true,
        };

        var act = () => parser.ParseTo<StringTest>(args);
        act.Should().ThrowExactly<InstanceMissingPropertyException>();

        parser.ThrowOnMissingProperty = false;
        act = () => parser.ParseTo<StringTest>(args);
        act.Should().NotThrow();
    }

    [Fact]
    public void Value_TooMany()
    {
        string[] args = ["abc", "def"];
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("Test", 0),
            ],
            ThrowOnTooManyValues = true,
        };

        var act = () => ParseToAnonymous(new { Test = (string?)null }, parser, args);
        act.Should().ThrowExactly<TooManyValuesException>()
            .Where(e => e.Argument == "def" && e.ArgumentPosition == 2 && e.MaxValuesExpected == 1 && e.UnexpectedValue == "def");
    }

    [Fact]
    public void ValueAndOption()
    {
        string[] args = ["abc", "--test", "def"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionCounter("Test", "test"),
            ],
            Values =
            [
                Value.Single("Value0", 0),
                Value.Single("Value1", 1),
            ],
            ThrowOnMissingProperty = false,
        };

        var result = ParseToAnonymous(
            new
            {
                Test = (int?)0,
                Value0 = (string?)null,
                Value1 = (string?)null,
            },
            parser,
            args);
        result.Test.Should().Be(1);
        result.Value0.Should().Be("abc");
        result.Value1.Should().Be("def");
    }

    [Fact]
    public void Enum()
    {
        string[] args = ["--test", "Tuesday"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Test", "test"),
            ],
        };

        var result = ParseToAnonymous(
            new
            {
                Test = (DayOfWeek?)null,
            },
            parser,
            args);
        result.Test.Should().Be(DayOfWeek.Tuesday);
    }

    [Fact]
    public void Bool()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (bool?)null }, parser, ["-v", "true"]);
        result.Value.Should().BeTrue();

        result = ParseToAnonymous(new { Value = (bool?)null }, parser, ["-v", "1"]);
        result.Value.Should().BeTrue();

        result = ParseToAnonymous(new { Value = (bool?)true }, parser, ["-v", "0"]);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public void Byte()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (byte?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void SByte()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (sbyte?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Char()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (char?)null }, parser, ["-v", "a"]);
        result.Value.Should().Be('a');
    }

    [Fact]
    public void Decimal()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (decimal?)null }, parser, ["-v", "4.5"]);
        result.Value.Should().Be(4.5m);
    }

    [Fact]
    public void Double()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (double?)null }, parser, ["-v", "4.5"]);
        result.Value.Should().Be(4.5d);
    }

    [Fact]
    public void Float()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (float?)null }, parser, ["-v", "4.5"]);
        result.Value.Should().Be(4.5f);
    }

    [Fact]
    public void Uint()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (uint?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Nint()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (nint?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be((nint)4);
    }

    [Fact]
    public void Nuint()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (nuint?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be((nuint)4);
    }

    [Fact]
    public void Long()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (long?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void ULong()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (ulong?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Short()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (short?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void UShort()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (ushort?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Int128()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (Int128?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be((Int128)4);
    }

    [Fact]
    public void Uint128()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (UInt128?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be((UInt128)4);
    }

    [Fact]
    public void Object()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (object?)null }, parser, ["-v", "4"]);
        result.Value.Should().Be("4");
    }

    [Fact]
    public void DateTime()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (DateTime?)null }, parser, ["-v", "2024-01-02 03:04:05"]);
        result.Value.Should().Be(new DateTime(2024, 1, 2, 3, 4, 5));
    }

    [Fact]
    public void DateTimeOffset()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (DateTimeOffset?)null }, parser, ["-v", "2024-01-02 03:04:05Z"]);
        result.Value.Should().Be(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero));
    }

    [Fact]
    public void DateOnly()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (DateOnly?)null }, parser, ["-v", "2024-01-02"]);
        result.Value.Should().Be(new DateOnly(2024, 1, 2));
    }

    [Fact]
    public void TimeOnly()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var result = ParseToAnonymous(new { Value = (TimeOnly?)null }, parser, ["-v", "03:04:05"]);
        result.Value.Should().Be(new TimeOnly(3, 4, 5));
    }

    [Fact]
    public void Unsupported()
    {
        var parser = new ArgumentParser
        {
            Options = [Option.ShortOptionWithValue("Value", 'v')],
        };

        var act = () => ParseToAnonymous(new { Value = (UnsupportedObj?)null }, parser, ["-v", "4"]);
        act.Should().ThrowExactly<InvalidArgumentValueDeserializationException>()
            .Where(e => e.Argument == "-v" && e.ArgumentPosition == 2 && e.DeserializationMessage != null);
    }

    [Fact]
    public void MultipleInstances()
    {
        string[] args = ["--test", "abc", "--test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            Commands =
            [
                new AbcCommand(),
            ],
        };

        var act = () => ParseToAnonymous(
            new
            {
                Test = false,
            },
            parser,
            args);
        act.Should().Throw<ArgumentParseException>()
            .Where(e => e.Argument == "abc" && e.ArgumentPosition == 2);
    }

    [Fact]
    public void MultipleInstances_NoRootInstance()
    {
        string[] args = ["abc", "--test"];
        var parser = new ArgumentParser
        {
            Commands =
            [
                new AbcCommand(),
            ],
        };

        var result = ParseToAnonymous(
            new
            {
                Test = false,
            },
            parser,
            args);
        result.Test.Should().BeTrue();
    }

    [Fact]
    public void ArgumentValuesOutOfRange()
    {
        string[] args = ["--test", "1", "2"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Test", "test", 3, 5),
            ],
        };

        var act = () => ParseToAnonymous(new { Test = (string[]?)null }, parser, args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--test"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 3
                && e.ExpectedMaxValues == 5
                && e.ActualValues == 2);
    }

    [Fact]
    public void ArgumentValuesOutOfRange_Infinite()
    {
        string[] args = ["--test", "1", "2"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Test", "test", 3, null),
            ],
        };

        var act = () => ParseToAnonymous(new { Test = (string[]?)null }, parser, args);
        act.Should().ThrowExactly<OptionValuesOutOfRangeException>()
            .Where(e => e.Argument == "--test"
                && e.ArgumentPosition == 1
                && e.ExpectedMinValues == 3
                && e.ExpectedMaxValues == null
                && e.ActualValues == 2);
    }

    [Fact]
    public void MissingProperty()
    {
        string[] args = ["--test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            ThrowOnMissingProperty = true,
        };

        var act = () => ParseToAnonymous(new { Prop = (bool?)null }, parser, args);
        act.Should().ThrowExactly<InstanceMissingPropertyException>()
            .Where(e => e.Argument == "--test" && e.ArgumentPosition == 1 && e.InstanceName != null && e.Property == "Test");
    }

    [Fact]
    public void MissingProperty_Ok()
    {
        string[] args = ["--test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Test", "test"),
            ],
            ThrowOnMissingProperty = false,
        };

        var result = ParseToAnonymous(new { Prop = (bool?)null }, parser, args);
        result.Prop.Should().BeNull();
    }

    [Fact]
    public void ValueAggregator()
    {
        string[] args = ["--test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Value", "test"),
            ],
        };

        var result = parser.ParseTo(args, null, [
            new DelegateArgumentValueAggregator<ValueAggregatorTestObj>(context =>
            {
                context.ArgumentToken.Argument.Should().Be("--test");
                context.InstanceValueHandler.SetValue(context.Instance, context.ValueName, 4);
                return true;
            }),
        ]);
        result.Value.Should().Be(4);
    }

    [Fact]
    public void BoolOptionNotSwitch()
    {
        string[] args = ["--test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("Test")
                {
                    LongNames = ["test"],
                },
            ],
        };

        var result = ParseToAnonymous(new { Test = (bool?)null }, parser, args);
        result.Test.Should().BeTrue();
    }

    private class BoolTest
    {
        public bool ValueA { get; set; }

        public bool? ValueB { get; set; }
    }

    private class IntTest
    {
        public int ValueA { get; set; }

        public int? ValueB { get; set; }
    }

    private class StringTest
    {
        public string? ValueA { get; set; }

        public string? ValueB { get; set; }
    }

    private class IntStringTest
    {
        public int? ValueA { get; set; }

        public string? ValueB { get; set; }
    }

    private class GenericTest<T>
    {
        public T? ValueA { get; set; }
    }

    private class StringConstructorTest
    {
        public ClassWithStringConstructor? Value { get; set; }
    }

    private class StringEnumerableConstructorTest
    {
        public ClassWithStringEnumerableConstructor? Value { get; set; }
    }

    private class ClassWithStringConstructor
    {
        public ClassWithStringConstructor(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    private class ClassWithStringEnumerableConstructor
    {
        public ClassWithStringEnumerableConstructor(IEnumerable<string> values)
        {
            Values = values;
        }

        public IEnumerable<string> Values { get; }
    }

    private class AbcCommand : Command
    {
        public AbcCommand()
        {
            Options = [Option.LongOptionSwitch("Test", "test")];
        }

        public override string Name => "abc";

        public override void Execute(ICommandExecutionContext context)
        {
        }
    }

    private class UnsupportedObj
    {
    }

    private class ValueAggregatorTestObj
    {
        public int? Value { get; set; }
    }
}
