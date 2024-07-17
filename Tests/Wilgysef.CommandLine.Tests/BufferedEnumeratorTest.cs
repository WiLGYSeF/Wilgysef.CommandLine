using FluentAssertions;

namespace Wilgysef.CommandLine.Tests;

public class BufferedEnumeratorTest
{
    [Fact]
    public void NormalEnumerator()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("ghi");
        enumerator.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void RollbackOne()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
    public void RollbackTwo()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
    public void RollbackToBeginning()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
    public void RollbackMultiple()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
    public void RollbackConsecutive()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
    public void RollbackDoesNotChangeUntilMove()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.Rollback(1);
        enumerator.Current.Should().Be("def");
    }

    [Fact]
    public void TryRollbackTooFar()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("def");
        enumerator.TryRollback(3).Should().Be(2);

        enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("abc");
        enumerator.TryRollback(1).Should().Be(1);
        enumerator.TryRollback(1).Should().Be(0);
    }

    [Fact]
    public void RollbackMoreThanPossibleFails()
    {
        var list = new List<string> { "abc", "def", "ghi" };
        var enumerator = new BufferedEnumerator<string>(list.GetEnumerator(), 2);

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
}
