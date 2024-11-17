using System.Collections;
using FluentAssertions;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public class ParseMultipleArgsCollectionTest : ParserBaseTest
{
    [Fact]
    public void Array()
    {
        MultipleArgsTest<StringArrayTest>(r => r.Value);
    }

    [Fact]
    public void IEnumerable()
    {
        MultipleArgsTest<StringIEnumerableTest>(r => r.Value);
    }

    [Fact]
    public void ICollection()
    {
        MultipleArgsTest<StringICollectionTest>(r => r.Value);
    }

    [Fact]
    public void IReadOnlyList()
    {
        MultipleArgsTest<StringIReadOnlyListTest>(r => r.Value);
    }

    [Fact]
    public void IList()
    {
        MultipleArgsTest<StringIListTest>(r => r.Value);
    }

    [Fact]
    public void IList_Nongeneric()
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<IListNongenericTest>(args);
        result.Value!.Count.Should().Be(2);
        result.Value[0].Should().Be("asdf");
        result.Value[1].Should().Be("1234");

        args = ["--str", "asdf", "1234"];
        parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Value", "str", 2, 2),
            ],
        };
        result = parser.ParseTo<IListNongenericTest>(args);
        result.Value!.Count.Should().Be(2);
        result.Value[0].Should().Be("asdf");
        result.Value[1].Should().Be("1234");
    }

    [Fact]
    public void List()
    {
        MultipleArgsTest<StringListTest>(r => r.Value);
    }

    [Fact]
    public void Queue()
    {
        MultipleArgsTest<StringQueueTest>(r => r.Value);
    }

    [Fact]
    public void Stack()
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<StringStackTest>(args);
        result.Value.Should().BeEquivalentTo(["1234", "asdf"], o => o.WithStrictOrdering());

        args = ["--str", "asdf", "1234"];
        parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Value", "str", 2, 2),
            ],
        };
        result = parser.ParseTo<StringStackTest>(args);
        result.Value.Should().BeEquivalentTo(["1234", "asdf"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void LinkedList()
    {
        MultipleArgsTest<StringLinkedListTest>(r => r.Value);
    }

    [Fact]
    public void ISet()
    {
        MultipleArgsTestSet<StringISetTest>(r => r.Value);
    }

    [Fact]
    public void IReadOnlySet()
    {
        MultipleArgsTestSet<StringIReadOnlySetTest>(r => r.Value);
    }

    [Fact]
    public void HashSet()
    {
        MultipleArgsTestSet<StringHashSetTest>(r => r.Value);
    }

    [Fact]
    public void SortedSet()
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<StringSortedSetTest>(args);
        result.Value.Should().BeEquivalentTo(["1234", "asdf"], o => o.WithStrictOrdering());

        args = ["--str", "asdf", "1234"];
        parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Value", "str", 2, 2),
            ],
        };
        result = parser.ParseTo<StringSortedSetTest>(args);
        result.Value.Should().BeEquivalentTo(["1234", "asdf"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void IntList()
    {
        string[] args = ["--str", "72", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<IntListTest>(args);
        result.Value.Should().BeEquivalentTo([72, 1234], o => o.WithStrictOrdering());
    }

    [Fact]
    public void ListDeserializer()
    {
        string[] args = ["--str", "72", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValues("Value", "str", 2),
            ],
            ListDeserializers = [new ListDeserializerTestObj()],
        };

        var result = parser.ParseTo<StringListTest>(args);
        result.Value.Should().BeEquivalentTo(["'72'", "'1234'"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void StringArray_FromSingle()
    {
        string[] args = ["--str", "1", "--str", "72", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Value", "str", 1, 2),
            ],
            ListDeserializers = [new ListDeserializerAggregatorTestObj()],
        };

        var result = parser.ParseTo<StringArrayTest>(args);
        result.Value.Should().BeEquivalentTo(["1", "72,1234"], o => o.WithStrictOrdering());
    }

    [Fact]
    public void IList_Aggregate_Single()
    {
        string[] args = ["--str", "abc", "--str", "test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var counter = 0;
        var result = parser.ParseTo(args, null, [
            new DelegateArgumentValueAggregator<IListNongenericTest>(context =>
            {
                if (++counter != 2)
                {
                    return false;
                }

                context.SetValue(context.ValueName, ((IEnumerable<object>)context.Value!).First());
                return true;
            })]);
        result.Value!.Count.Should().Be(2);
        result.Value[0].Should().Be("abc");
        result.Value[1].Should().Be("test");
    }

    [Fact]
    public void StringICollection_Aggregate_Single_Null()
    {
        string[] args = ["--str", "abc", "--str", "test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var counter = 0;
        var result = parser.ParseTo(args, null, [
            new DelegateArgumentValueAggregator<StringICollectionTest>(context =>
            {
                if (++counter != 2)
                {
                    return false;
                }

                context.SetValue(context.ValueName, null);
                return true;
            })]);
        result.Value!.Count.Should().Be(2);
        result.Value.ElementAt(0).Should().Be("abc");
        result.Value.ElementAt(1).Should().BeNull();
    }

    [Fact]
    public void StringICollection_Aggregate_Single_String()
    {
        string[] args = ["--str", "abc", "--str", "test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var counter = 0;
        var result = parser.ParseTo(args, null, [
            new DelegateArgumentValueAggregator<StringICollectionTest>(context =>
            {
                if (++counter != 2)
                {
                    return false;
                }

                context.SetValue(context.ValueName, "hello");
                return true;
            })]);
        result.Value!.Count.Should().Be(2);
        result.Value.ElementAt(0).Should().Be("abc");
        result.Value.ElementAt(1).Should().Be("hello");
    }

    [Fact]
    public void StringICollection_Aggregate_Single_IEnumerableNongeneric()
    {
        string[] args = ["--str", "abc", "--str", "test"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var counter = 0;
        var result = parser.ParseTo(args, null, [
            new DelegateArgumentValueAggregator<StringICollectionTest>(context =>
            {
                if (++counter != 2)
                {
                    return false;
                }

                context.SetValue(context.ValueName, new ArrayList { "hello" });
                return true;
            })]);
        result.Value!.Count.Should().Be(2);
        result.Value.ElementAt(0).Should().Be("abc");
        result.Value.ElementAt(1).Should().Be("hello");
    }

    private static void MultipleArgsTest<TInstance>(Func<TInstance, IEnumerable<string>?> func)
        where TInstance : class
    {
        string[] args = ["--str", "asdf", "--str", "1234"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<TInstance>(args);
        func(result).Should().BeEquivalentTo(["asdf", "1234"], o => o.WithStrictOrdering());

        args = ["--str", "asdf", "1234"];
        parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValueRange("Value", "str", 2, 2),
            ],
        };
        result = parser.ParseTo<TInstance>(args);
        func(result).Should().BeEquivalentTo(["asdf", "1234"], o => o.WithStrictOrdering());
    }

    private static void MultipleArgsTestSet<TInstance>(Func<TInstance, IEnumerable<string>?> func)
        where TInstance : class
    {
        MultipleArgsTest(func);

        string[] args = ["--str", "asdf", "--str", "asdf"];
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionWithValue("Value", "str"),
            ],
        };

        var result = parser.ParseTo<TInstance>(args);
        func(result).Should().BeEquivalentTo(["asdf"], o => o.WithStrictOrdering());
    }

    private class StringArrayTest
    {
        public string[]? Value { get; set; }
    }

    private class StringIEnumerableTest
    {
        public IEnumerable<string>? Value { get; set; }
    }

    private class StringICollectionTest
    {
        public ICollection<string>? Value { get; set; }
    }

    private class StringIReadOnlyListTest
    {
        public IReadOnlyList<string>? Value { get; set; }
    }

    private class StringIListTest
    {
        public IList<string>? Value { get; set; }
    }

    private class IListNongenericTest
    {
        public IList? Value { get; set; }
    }

    private class StringListTest
    {
        public List<string>? Value { get; set; }
    }

    private class StringQueueTest
    {
        public Queue<string>? Value { get; set; }
    }

    private class StringStackTest
    {
        public Stack<string>? Value { get; set; }
    }

    private class StringLinkedListTest
    {
        public LinkedList<string>? Value { get; set; }
    }

    private class StringISetTest
    {
        public ISet<string>? Value { get; set; }
    }

    private class StringIReadOnlySetTest
    {
        public IReadOnlySet<string>? Value { get; set; }
    }

    private class StringHashSetTest
    {
        public HashSet<string>? Value { get; set; }
    }

    private class StringSortedSetTest
    {
        public SortedSet<string>? Value { get; set; }
    }

    private class IntListTest
    {
        public List<int>? Value { get; set; }
    }

    private class ListDeserializerTestObj : ArgumentValueListDeserializerStrategy
    {
        public override bool Deserialize(Context context, out object? result)
        {
            context.Type.Should().Be(typeof(List<string?>));
            context.ArgumentToken.Option!.Name.Should().Be("Value");
            context.ValueName.Should().Be("Value");
            context.KeepFirstValue.Should().BeFalse();

            result = context.Values.Select(v => $"'{v}'").ToList();
            return true;
        }
    }

    private class ListDeserializerAggregatorTestObj : ArgumentValueListDeserializerStrategy
    {
        public override bool Deserialize(Context context, out object? result)
        {
            if (context.Values.Count == 1)
            {
                result = null;
                return false;
            }

            result = string.Join(",", context.Values);
            return true;
        }
    }

    private class IListAggregateSingleAggregator : ArgumentValueAggregator
    {
        public override bool MatchesInstanceType(Type type)
            => true;

        public override bool SetValue(ArgumentValueAggregatorContext context)
            => throw new NotImplementedException();
    }
}
