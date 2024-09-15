using FluentAssertions;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.Parsers.TokenizerTests;

public class ValidationTest
{
    [Fact]
    public void Option_EmptyName()
    {
        var act = () => new Option("");
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Option_NoNames()
    {
        var option = new Option("test");
        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_EmptyLongNames()
    {
        var option = new Option("test")
        {
            LongNames = ["a", ""],
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_EmptyShortPrefix()
    {
        var option = new Option("test")
        {
            ShortNames = ['a'],
            ShortNamePrefix = "",
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_EmptyLongPrefix()
    {
        var option = new Option("test")
        {
            LongNames = ["a"],
            LongNamePrefix = "",
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_EmptyKeyValueSeparator()
    {
        var option = new Option("test")
        {
            ShortNames = ['a'],
            KeyValueSeparators = [""],
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_InvalidValuesAndSwitch()
    {
        var option = new Option("test")
        {
            LongNames = ["a"],
            ValueCountRange = new ValueRange(3, 1),
            Switch = true,
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_InvalidValuesAndCounter()
    {
        var option = new Option("test")
        {
            LongNames = ["a"],
            ValueCountRange = new ValueRange(3, 1),
            Counter = true,
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_InvalidSwitchAndCounter()
    {
        var option = new Option("test")
        {
            LongNames = ["a"],
            Switch = true,
            Counter = true,
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void Option_InvalidRange()
    {
        var option = new Option("test")
        {
            LongNames = ["a"],
            ValueCountRange = new ValueRange(3, 1),
        };

        var act = () => option.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void RequiredOption()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("test")
                {
                    LongNames = ["a"],
                    Required = true,
                },
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<OptionGroupOptionMismatchException>()
            .Where(e => e.OptionGroup == null
                && e.Options.Count == 1
                && e.Options.First().Name == "test"
                && e.ArgumentTokens.Count == 0
                && e.MissingRequiredOption
                && !e.MutuallyExclusiveOptions);

        var result = parser.Tokenize(["--a"]);
        var group = result.ArgumentGroups[0];
        group.Arguments.Should().HaveCount(1);
    }

    [Fact]
    public void MutuallyExclusiveOptions()
    {
        string[] args = ["-a", "-b"];
        var parser = new ArgumentParser
        {
            OptionGroups = [OptionGroup.AtMostOne("test")],
            Options =
            [
                new Option("ValueA")
                {
                    ShortNames = ['a'],
                    GroupNames = ["test"],
                },
                new Option("ValueB")
                {
                    ShortNames = ['b'],
                    GroupNames = ["test"],
                },
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionGroupOptionMismatchException>()
            .Where(e => e.OptionGroup!.Name == "test"
                && e.Options.Count == 2
                && e.ArgumentTokens.Count == 2
                && !e.MissingRequiredOption
                && e.MutuallyExclusiveOptions);

        var result = parser.Tokenize(["-a"]);
        var group = result.ArgumentGroups[0];
        group.Arguments.Should().HaveCount(1);
    }

    [Fact]
    public void UniqueOption()
    {
        string[] args = ["-a", "-a"];
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("ValueA")
                {
                    ShortNames = ['a'],
                    Unique = true,
                },
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<MultipleUniqueOptionException>()
            .Where(e => e.Argument == "-a" && e.ArgumentPosition == 1 && e.OtherArgumentPosition == 2);

        var result = parser.Tokenize(["-a"]);
        var group = result.ArgumentGroups[0];
        group.Arguments.Should().HaveCount(1);
    }

    [Fact]
    public void GroupOptions()
    {
        string[] args = [];
        var parser = new ArgumentParser
        {
            OptionGroups = [OptionGroup.AtLeastOne("test")],
            Options =
            [
                new Option("ValueA")
                {
                    ShortNames = ['a'],
                    GroupNames = ["test"],
                },
                new Option("ValueB")
                {
                    ShortNames = ['b'],
                    GroupNames = ["test"],
                },
            ],
        };

        var act = () => parser.Tokenize(args);
        act.Should().ThrowExactly<OptionGroupOptionMismatchException>()
            .Where(e => e.OptionGroup!.Name == "test"
                && e.Options.Count == 2
                && e.ArgumentTokens.Count == 0
                && e.MissingRequiredOption
                && !e.MutuallyExclusiveOptions);

        var result = parser.Tokenize(["-a"]);
        var group = result.ArgumentGroups[0];
        group.Arguments.Should().HaveCount(1);
    }

    [Fact]
    public void Value_InvalidName()
    {
        var act = () => new Value("", new ValueRange(1, 1));
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Value_InvalidRange()
    {
        var value = new Value("test", new ValueRange(3, 1));

        var act = () => value.Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void ShortNamePrefix_Empty()
    {
        var parser = new ArgumentParser();
        parser.ShortNamePrefixDefault = "";

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<InvalidOptionException>();
    }

    [Fact]
    public void LongNamePrefix_Empty()
    {
        var parser = new ArgumentParser();
        parser.LongNamePrefixDefault = "";

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<InvalidOptionException>();
    }

    [Fact]
    public void KeyValueSeparator_Empty()
    {
        var parser = new ArgumentParser();
        parser.KeyValueSeparatorsDefault = [""];

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<InvalidOptionException>();
    }

    [Fact]
    public void Option_Duplicate()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("Option", "a"),
                Option.LongOptionSwitch("Option", "b"),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "Option" && e.OtherOptionName == "Option");
    }

    [Fact]
    public void ShortOption_Duplicate()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.ShortOptionSwitch("OptionA", 'a'),
                Option.ShortOptionSwitch("OptionB", 'a'),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "OptionB" && e.OtherOptionName == "OptionA");
    }

    [Fact]
    public void LongOption_Duplicate()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                Option.LongOptionSwitch("OptionA", "a"),
                Option.LongOptionSwitch("OptionB", "a"),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "OptionB" && e.OtherOptionName == "OptionA");
    }

    [Fact]
    public void LongOption_Duplicate_ByPrefix()
    {
        var parser = new ArgumentParser
        {
            Options =
            [
                new Option("OptionA")
                {
                    LongNamePrefix = "--a",
                    LongNames = ["a"],
                },
                Option.LongOptionSwitch("OptionB", "aa"),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "OptionB" && e.OtherOptionName == "OptionA");
    }

    [Fact]
    public void Value_Duplicate()
    {
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("ValueA", 0),
                Value.Single("ValueA", 1),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "ValueA" && e.OtherOptionName == "ValueA");
    }

    [Fact]
    public void Value_RangeOverlap_Single()
    {
        var parser = new ArgumentParser
        {
            Values =
            [
                Value.Single("ValueA", 0),
                Value.Single("ValueB", 0),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "ValueB");
    }

    [Fact]
    public void Value_RangeOverlap()
    {
        var parser = new ArgumentParser
        {
            Values =
            [
                new Value("ValueA", new ValueRange(0, 2)),
                new Value("ValueB", new ValueRange(2, 3)),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "ValueB");
    }

    [Fact]
    public void Command_Duplicate()
    {
        var parser = new ArgumentParser();
        parser.AddCommand("Command", context => { });
        parser.AddCommand("Command", context => { });

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "Command" && e.OtherOptionName == "Command");
    }

    [Fact]
    public void Command_Duplicate_Alias()
    {
        var parser = new ArgumentParser();
        parser.AddCommand("Command", context => { }, cmd =>
        {
            cmd.Aliases = ["Cmd"];
        });
        parser.AddCommand("Cmd", context => { });

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "Cmd" && e.OtherOptionName == "Cmd");
    }

    [Fact]
    public void OptionGroup_Name()
    {
        var act = () => new OptionGroup("", 3, 1).Validate();
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void OptionGroup_Range()
    {
        var act = () => new OptionGroup("test", 3, 1).Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");

        act = () => new OptionGroup("test", null, 1).Validate();
        act.Should().ThrowExactly<InvalidOptionException>()
            .Where(e => e.OptionName == "test");
    }

    [Fact]
    public void OptionGroup_Duplicate()
    {
        var parser = new ArgumentParser
        {
            OptionGroups =
            [
                OptionGroup.ExactlyOne("test"),
                OptionGroup.ExactlyOne("test"),
            ],
        };

        var act = () => parser.Tokenize([]);
        act.Should().ThrowExactly<DuplicateOptionException>()
            .Where(e => e.OptionName == "test" && e.OtherOptionName == "test");
    }

    [Fact]
    public void OptionGroup_Mismatch()
    {
        var parser = new ArgumentParser
        {
            OptionGroups =
            [
                new OptionGroup("test", 2, 3),
            ],
            Options =
            [
                new Option("ValueA")
                {
                    ShortNames = ['a'],
                    GroupNames = ["test"],
                },
                new Option("ValueB")
                {
                    ShortNames = ['b'],
                    GroupNames = ["test"],
                },
                new Option("ValueC")
                {
                    ShortNames = ['c'],
                    GroupNames = ["test"],
                },
            ],
        };

        var act = () => parser.Tokenize(["-a"]);
        act.Should().ThrowExactly<OptionGroupOptionMismatchException>()
            .Where(e => e.OptionGroup!.Name == "test"
                && e.Options.Count == 3
                && e.ArgumentTokens.Count == 1);

        var result = parser.Tokenize(["-ab"]);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(2);
    }

    [Fact]
    public void OptionGroup_All()
    {
        var parser = new ArgumentParser
        {
            OptionGroups = [OptionGroup.All("test")],
            Options =
            [
                new Option("ValueA")
                {
                    ShortNames = ['a'],
                    GroupNames = ["test"],
                },
                new Option("ValueB")
                {
                    ShortNames = ['b'],
                    GroupNames = ["test"],
                },
                new Option("ValueC")
                {
                    ShortNames = ['c'],
                    GroupNames = ["test"],
                },
            ],
        };

        var act = () => parser.Tokenize(["-a"]);
        act.Should().ThrowExactly<OptionGroupOptionMismatchException>()
            .Where(e => e.OptionGroup!.Name == "test"
                && e.Options.Count == 3
                && e.ArgumentTokens.Count == 1);

        var result = parser.Tokenize(["-abc"]);
        result.ArgumentGroups[0].Arguments.Should().HaveCount(3);
    }
}
