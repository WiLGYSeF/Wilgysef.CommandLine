using FluentAssertions;

namespace Wilgysef.CommandLine.Tests;

public class BufferedEnumeratorTest
{
    [Fact]
    public void NormalEnumerator()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_One()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(1);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_Two()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(2);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_ToBeginning()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(2);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_Multiple()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(2);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(2);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_Consecutive()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(1);
        enumerator.Rollback(1);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void Rollback_DoesNotChangeUntilMove()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(1);
        enumerator.Current.Should().Be("def");
    }

    [Fact]
    public void TryRollback_TooFar()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using (var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2))
        {
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be("abc");
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be("def");
            enumerator.TryRollback(3).Should().Be(2);
        }

        using (var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2))
        {
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be("abc");
            enumerator.TryRollback(1).Should().Be(1);
            enumerator.TryRollback(1).Should().Be(0);
        }
    }

    [Fact]
    public void Rollback_MoreThanPossibleFails()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");

        var act = () => enumerator.Rollback(2);
        act.Should().Throw<Exception>();

        enumerator.Reset();

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");

        act = () => enumerator.Rollback(3);
        act.Should().Throw<Exception>();

        enumerator.Rollback(1);
        act = () => enumerator.Rollback(2);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void IEnumeratorCurrent()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);
        enumerator.MoveNext();
        enumerator.Current.Should().Be("abc");
        ((System.Collections.IEnumerator)enumerator).Current.Should().Be("abc");
    }

    [Fact]
    public void InvalidBuffer()
    {
        var act = () => new BufferedEnumerator<string>(Enumerable.Empty<string>().GetEnumerator(), 0);
        act.Should().ThrowExactly<ArgumentException>();

        act = () => new BufferedEnumerator<string>(Enumerable.Empty<string>().GetEnumerator(), -1);
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void TryRollback_InvalidCount()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        using var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        var act = () => enumerator.TryRollback(-2);
        act.Should().Throw<ArgumentException>();
    }
}
