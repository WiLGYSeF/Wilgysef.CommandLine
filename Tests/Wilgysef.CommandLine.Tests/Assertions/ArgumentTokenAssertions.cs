using FluentAssertions;
using FluentAssertions.Primitives;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.Assertions;

public class ArgumentTokenAssertions : ReferenceTypeAssertions<ArgumentToken, ArgumentTokenAssertions>
{
    public ArgumentTokenAssertions(ArgumentToken subject)
        : base(subject)
    {
    }

    protected override string Identifier => "argument";

    [CustomAssertion]
    public AndConstraint<ArgumentTokenAssertions> BeValue(string value, string because = "", params object[] becauseArgs)
    {
        Subject.Option.Should().BeNull();
        Subject.Values.Should().HaveCount(1);
        Subject.Values![0].Should().Be(value);

        return new AndConstraint<ArgumentTokenAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<ArgumentTokenAssertions> BeOptionWithoutValues(string name, string because = "", params object[] becauseArgs)
    {
        Subject.Option!.Name.Should().Be(name);
        Subject.Values.Should().BeNull();

        return new AndConstraint<ArgumentTokenAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<ArgumentTokenAssertions> BeOptionWithValue(string name, string value, string because = "", params object[] becauseArgs)
    {
        Subject.Option!.Name.Should().Be(name);
        Subject.Values.Should().HaveCount(1);
        Subject.Values![0].Should().Be(value);

        return new AndConstraint<ArgumentTokenAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<ArgumentTokenAssertions> BeOptionWithValues(string name, IEnumerable<string> values, string because = "", params object[] becauseArgs)
    {
        Subject.Option!.Name.Should().Be(name);
        Subject.Values.Should().BeEquivalentTo(values, o => o.WithStrictOrdering());

        return new AndConstraint<ArgumentTokenAssertions>(this);
    }
}

#pragma warning disable SA1402 // File may only contain a single type
public static class ArgumentTokenExtensions
#pragma warning restore SA1402 // File may only contain a single type
{
    public static ArgumentTokenAssertions Should(this ArgumentToken subject)
    {
        return new ArgumentTokenAssertions(subject);
    }
}
