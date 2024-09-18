using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Argument tokenizer.
/// </summary>
/// <param name="argumentParser">Argument parser.</param>
internal class Tokenizer(ArgumentParser argumentParser)
{
    private ICollection<Option> Options { get; set; } = argumentParser.Options;

    private ICollection<OptionGroup> OptionGroups { get; set; } = argumentParser.OptionGroups;

    private ICollection<Value> Values { get; set; } = argumentParser.Values;

    private ICollection<ICommand> Commands { get; set; } = argumentParser.Commands;

    private string ShortNamePrefixDefault { get; set; } = argumentParser.ShortNamePrefixDefault;

    private string LongNamePrefixDefault { get; set; } = argumentParser.LongNamePrefixDefault;

    private ICollection<string> KeyValueSeparatorsDefault { get; set; } = argumentParser.KeyValueSeparatorsDefault;

    private bool ShortNameImmediateValue { get; set; } = argumentParser.ShortNameImmediateValue;

    private bool LongNameCaseInsensitiveDefault { get; set; } = argumentParser.LongNameCaseInsensitiveDefault;

    private bool KeepFirstValue { get; set; } = argumentParser.KeepFirstValue;

    private string ArgumentLiteralSeparator { get; set; } = argumentParser.ArgumentLiteralSeparator;

    private bool ThrowOnArgumentUnexpectedKeyValue { get; set; } = argumentParser.ThrowOnArgumentUnexpectedKeyValue;

    private bool ThrowOnUnknownOptions { get; set; } = argumentParser.ThrowOnUnknownOptions;

    private bool IgnoreUnknownOptions { get; set; } = argumentParser.IgnoreUnknownOptions;

    private bool ThrowOnUnknownShortOptions { get; set; } = argumentParser.ThrowOnUnknownShortOptions;

    /// <summary>
    /// Tokenizes arguments.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <returns>Tokenized arguments.</returns>
    public TokenizedArguments Tokenize(IEnumerable<string> args)
    {
        var argGroups = new List<ArgumentTokenGroup>();
        var argsEnumerator = new BufferedEnumerator<string>(args.GetEnumerator(), 1);
        var position = 1;
        var parseLiteral = false;
        CommandMatch? lastCommand = null;
        List<Option>? helpOptionLongOption = null;
        Trie<Option>? helpOptionPrefixTrie = null;

        if (argumentParser.HelpOption != null)
        {
            argumentParser.HelpOption.Validate();

            if (argumentParser.HelpOption.HasShortNames)
            {
                helpOptionPrefixTrie = new();
                helpOptionPrefixTrie.Add(argumentParser.HelpOption.ShortNamePrefix ?? ShortNamePrefixDefault, argumentParser.HelpOption);
            }

            if (argumentParser.HelpOption.HasLongNames)
            {
                helpOptionLongOption = [argumentParser.HelpOption];
            }
        }

        bool brokeEarly;
        do
        {
            var longOptions = new List<Option>();
            var shortOptions = new List<Option>();
            var shortOptionPrefixTrie = new Trie<Option>();

            ValidateOptionsValuesCommands(longOptions, shortOptions, shortOptionPrefixTrie);

            var argTokens = new List<ArgumentToken>();
            CommandMatch? foundCommand = null;

            for (brokeEarly = false; argsEnumerator.MoveNext(); position++)
            {
                var arg = argsEnumerator.Current;

                if (parseLiteral)
                {
                    argTokens.Add(ArgumentToken.Unparsed(arg, position));
                    continue;
                }

                if (arg == ArgumentLiteralSeparator)
                {
                    parseLiteral = true;
                    continue;
                }

                if (TokenizeArg(argsEnumerator, longOptions, shortOptionPrefixTrie, arg, ref position, argTokens))
                {
                    continue;
                }

                if (MatchesCommand(Commands, arg, out var command))
                {
                    foundCommand = new CommandMatch(command, arg, position);
                    brokeEarly = true;
                    position++;
                    break;
                }

                if (helpOptionLongOption != null
                    && TokenizeArg(argsEnumerator, helpOptionLongOption, helpOptionPrefixTrie!, arg, ref position, argTokens))
                {
                    continue;
                }

                var handled = false;

                if (ArgumentStartsWithDefaultOptionPrefixes(arg))
                {
                    if (ThrowOnUnknownOptions)
                    {
                        throw new UnknownOptionException(arg, position, Options);
                    }

                    if (IgnoreUnknownOptions)
                    {
                        continue;
                    }

                    handled = true;
                }

                if (!handled && Commands.Count > 0 && Values.Count == 0)
                {
                    throw new UnknownCommandException(arg, position, Commands);
                }

                argTokens.Add(ArgumentToken.Unparsed(arg, position));
            }

            ParsePostValidation(argTokens);

            if (foundCommand != null)
            {
                UseCommandContext(foundCommand.Command);
            }

            argGroups.Add(new ArgumentTokenGroup(argTokens, lastCommand));
            lastCommand = foundCommand;
        }
        while (brokeEarly);

        return new TokenizedArguments(argGroups);

        static bool MatchesCommand(IEnumerable<ICommand> commands, string arg, [NotNullWhen(true)] out ICommand? command)
        {
            foreach (var cmd in commands)
            {
                // TODO: should we check all commands for matches?
                if (cmd.Matches(arg))
                {
                    command = cmd;
                    return true;
                }
            }

            command = null;
            return false;
        }
    }

    /// <summary>
    /// Applies options from command.
    /// </summary>
    /// <param name="command">Command.</param>
    private void ApplyOptions(ICommand command)
    {
        SetIfNotNull(x => x.ShortNamePrefixDefault, command.ShortNamePrefix);
        SetIfNotNull(x => x.LongNamePrefixDefault, command.LongNamePrefix);
        SetIfNotNull(x => x.KeyValueSeparatorsDefault, command.KeyValueSeparators);
        SetIfNotNull(x => x.ShortNameImmediateValue, command.ShortNameImmediateValue);
        SetIfNotNull(x => x.LongNameCaseInsensitiveDefault, command.LongNameCaseInsensitive);
        SetIfNotNull(x => x.KeepFirstValue, command.KeepFirstValue);
        SetIfNotNull(x => x.ArgumentLiteralSeparator, command.ArgumentLiteralSeparator);
        SetIfNotNull(x => x.ThrowOnArgumentUnexpectedKeyValue, command.ThrowOnArgumentUnexpectedKeyValue);
        SetIfNotNull(x => x.ThrowOnUnknownOptions, command.ThrowOnUnknownOptions);
        SetIfNotNull(x => x.IgnoreUnknownOptions, command.IgnoreUnknownOptions);
        SetIfNotNull(x => x.ThrowOnUnknownShortOptions, command.ThrowOnUnknownShortOptions);

        void SetIfNotNull<T>(Expression<Func<Tokenizer, T>> expression, T? value)
        {
            if (value != null)
            {
                ((PropertyInfo)((MemberExpression)expression.Body).Member).SetValue(this, value);
            }
        }
    }

    private bool TokenizeArg(
        BufferedEnumerator<string> argsEnumerator,
        List<Option> longOptions,
        Trie<Option> shortOptionPrefixTrie,
        string arg,
        ref int argPos,
        List<ArgumentToken> argTokens)
    {
        foreach (var option in longOptions)
        {
            var argToken = TokenizeLongOption(option, arg, ref argPos, argsEnumerator);
            if (argToken != null)
            {
                argTokens.Add(argToken);
                return true;
            }
        }

        if (arg.StartsWith(LongNamePrefixDefault))
        {
            return false;
        }

        var options = shortOptionPrefixTrie.GetValues(arg, out var triePrefix);
        if (options != null)
        {
            var shortArgToken = TokenizeShortOption(options, arg, ref argPos, arg[..triePrefix!.Value], argsEnumerator);
            if (shortArgToken != null)
            {
                argTokens.AddRange(shortArgToken);
                return true;
            }
        }

        return false;
    }

    private ArgumentToken? TokenizeLongOption(
        Option option,
        string arg,
        ref int argPos,
        BufferedEnumerator<string> argsEnumerator)
    {
        if (!MatchesLongOption(option, arg, out var argMatch, out var value))
        {
            return null;
        }

        if (!option.CanHaveValues)
        {
            if (ThrowOnArgumentUnexpectedKeyValue && value != null)
            {
                throw new OptionUnexpectedKeyValueException(arg, argPos);
            }

            return ArgumentToken.NoValue(option, arg, argPos, argMatch);
        }

        var values = new List<string>();
        if (value != null)
        {
            values.Add(value);
        }

        var argStartPos = argPos;
        argPos += CollectArgumentValues(option, values.Count, argsEnumerator, values);

        if (!option.ValueCountRange!.InRange(values.Count))
        {
            throw new OptionValuesOutOfRangeException(arg, argStartPos, option.ValueCountRange.Min, option.ValueCountRange.Max, values.Count);
        }

        return ArgumentToken.WithValues(option, arg, argPos, argMatch, values);
    }

    private List<ArgumentToken>? TokenizeShortOption(
        IEnumerable<Option> options,
        string arg,
        ref int argPos,
        string prefix,
        BufferedEnumerator<string> argsEnumerator)
    {
        var argTokens = new List<ArgumentToken>();

        for (var i = prefix.Length; i < arg.Length; i++)
        {
            var shortOpt = arg[i];
            var option = MatchOption(shortOpt);
            if (option == null)
            {
                if (ThrowOnUnknownShortOptions)
                {
                    throw new UnknownShortOptionException(arg, argPos, shortOpt, Options);
                }

                continue;
            }

            var keyValueSeparators = option.KeyValueSeparators ?? KeyValueSeparatorsDefault;
            var keyValue = MatchesKeyValueSeparator(arg.AsSpan(i + 1), keyValueSeparators, out var separator)
                ? arg[(i + 1 + separator.Length)..]
                : null;
            if (!option.CanHaveValues)
            {
                if (keyValue != null)
                {
                    if (ThrowOnArgumentUnexpectedKeyValue)
                    {
                        throw new ShortOptionUnexpectedKeyValueException(arg, argPos, shortOpt);
                    }

                    i += separator!.Length + keyValue.Length;
                }

                argTokens.Add(ArgumentToken.NoValue(option, arg, argPos, shortOpt.ToString()));
                continue;
            }

            var values = new List<string>();

            if (keyValue != null)
            {
                values.Add(keyValue);
            }
            else if (i < arg.Length - 1)
            {
                if (!(option.ShortNameImmediateValue ?? ShortNameImmediateValue))
                {
                    throw new ShortOptionUnexpectedKeyValueException(arg, argPos, shortOpt);
                }

                values.Add(arg[(i + 1)..]);
            }

            var argStartPos = argPos;
            argPos += CollectArgumentValues(option, values.Count, argsEnumerator, values);

            if (!option.ValueCountRange!.InRange(values.Count))
            {
                throw new OptionValuesOutOfRangeException(arg, argStartPos, option.ValueCountRange.Min, option.ValueCountRange.Max, values.Count);
            }

            argTokens.Add(ArgumentToken.WithValues(option, arg, argPos, shortOpt.ToString(), values));
            break;
        }

        return argTokens;

        Option? MatchOption(char ch)
        {
            foreach (var option in options)
            {
                if (option.MatchesShortName(ch, out _))
                {
                    return option;
                }
            }

            return null;
        }
    }

    private int CollectArgumentValues(
        Option option,
        int argsCollected,
        BufferedEnumerator<string> argsEnumerator,
        List<string> values)
    {
        var argsMoved = 0;
        while (option.ValueCountRange!.UnderMax(argsCollected) && argsEnumerator.MoveNext())
        {
            argsMoved++;

            var arg = argsEnumerator.Current;
            if (arg == ArgumentLiteralSeparator)
            {
                return argsMoved;
            }

            // TODO: how to escape matches?

            foreach (var otherOpt in Options)
            {
                if (MatchesOption(otherOpt, arg))
                {
                    argsEnumerator.Rollback(1);
                    return argsMoved - 1;
                }
            }

            foreach (var command in Commands)
            {
                if (command.Matches(arg))
                {
                    argsEnumerator.Rollback(1);
                    return argsMoved - 1;
                }
            }

            if (ArgumentStartsWithDefaultOptionPrefixes(arg))
            {
                argsEnumerator.Rollback(1);
                return argsMoved - 1;
            }

            values.Add(arg);
            argsCollected++;
        }

        return argsMoved;
    }

    private bool MatchesOption(Option option, string arg)
    {
        return MatchesLongOption(option, arg, out _, out _)
            || MatchesShortOption(option, arg, out _);
    }

    private bool MatchesLongOption(Option option, string arg, [NotNullWhen(true)] out string? argMatch, out string? value)
    {
        var longPrefix = option.LongNamePrefix ?? LongNamePrefixDefault;

        if (!option.HasLongNames || !arg.StartsWith(longPrefix))
        {
            argMatch = null;
            value = null;
            return false;
        }

        var keyValueSeparators = option.KeyValueSeparators ?? KeyValueSeparatorsDefault;
        var keyValueSepIdx = IndexOfKeyValueSeparator(arg, keyValueSeparators, out var separator);
        if (keyValueSepIdx == -1)
        {
            keyValueSepIdx = arg.Length;
        }

        var argSubstring = arg[longPrefix.Length..keyValueSepIdx];
        if (argSubstring.Length == 0)
        {
            argMatch = null;
            value = null;
            return false;
        }

        argMatch = argSubstring;
        value = keyValueSepIdx < arg.Length
            ? arg[(keyValueSepIdx + separator!.Length)..]
            : null;
        return option.MatchesLongName(argSubstring, out _, LongNameCaseInsensitiveDefault);
    }

    private bool MatchesShortOption(Option option, string arg, out string? value)
    {
        var shortPrefix = option.ShortNamePrefix ?? ShortNamePrefixDefault;
        if (!option.HasShortNames || !arg.StartsWith(shortPrefix))
        {
            value = null;
            return false;
        }

        var keyValueSeparators = option.KeyValueSeparators ?? KeyValueSeparatorsDefault;
        var keyValueSepIdx = IndexOfKeyValueSeparator(arg, keyValueSeparators, out var separator);
        if (keyValueSepIdx == -1)
        {
            keyValueSepIdx = arg.Length;
        }

        var argSubstring = arg[shortPrefix.Length..keyValueSepIdx];
        if (argSubstring.Length == 0)
        {
            value = null;
            return false;
        }

        var matchIdx = -1;
        for (var i = 0; i < argSubstring.Length; i++)
        {
            if (option.ShortNames!.Any(n => n == argSubstring[i]))
            {
                matchIdx = i;
                break;
            }
        }

        if (matchIdx == -1)
        {
            value = null;
            return false;
        }

        for (var i = 0; i < matchIdx; i++)
        {
            foreach (var otherOpt in Options)
            {
                if (otherOpt.HasShortNames
                    && otherOpt.ShortNames!.Any(n => n == argSubstring[i])
                    && otherOpt.ValueCountRange?.Min >= 1)
                {
                    value = null;
                    return false;
                }
            }
        }

        if (option.ValueCountRange?.Min >= 1
            && !arg.AsSpan(shortPrefix.Length + matchIdx + 1).StartsWith(separator))
        {
            value = matchIdx + 1 < argSubstring.Length
                ? argSubstring[(matchIdx + 1)..]
                : null;
        }
        else
        {
            value = keyValueSepIdx < arg.Length
                ? arg[(keyValueSepIdx + separator!.Length)..]
                : null;
        }

        return true;
    }

    private static bool MatchesKeyValueSeparator(ReadOnlySpan<char> span, IEnumerable<string> separators, [NotNullWhen(true)] out string? separator)
    {
        foreach (var sep in separators)
        {
            if (span.StartsWith(sep))
            {
                separator = sep;
                return true;
            }
        }

        separator = null;
        return false;
    }

    private static int IndexOfKeyValueSeparator(ReadOnlySpan<char> span, IEnumerable<string> separators, [NotNullWhen(true)] out string? separator)
    {
        foreach (var sep in separators)
        {
            var idx = span.IndexOf(sep);
            if (idx != -1)
            {
                separator = sep;
                return idx;
            }
        }

        separator = null;
        return -1;
    }

    private void ValidateOptionsValuesCommands(
        List<Option> longOptionsList,
        List<Option> shortOptionsList,
        Trie<Option> shortOptionPrefixTrie)
    {
        if (ShortNamePrefixDefault.Length == 0)
        {
            throw new InvalidOptionException("Default short option prefix cannot be empty");
        }

        if (LongNamePrefixDefault.Length == 0)
        {
            throw new InvalidOptionException("Default long option prefix cannot be empty");
        }

        if (KeyValueSeparatorsDefault?.Any(string.IsNullOrEmpty) ?? true)
        {
            throw new InvalidOptionException("Default key-value separator cannot be empty");
        }

        var names = new HashSet<string>();
        var optionsDict = new Dictionary<string, Option>();
        var optionGroups = new Dictionary<string, OptionGroup>();

        foreach (var group in OptionGroups)
        {
            group.Validate();

            if (!optionGroups.TryAdd(group.Name, group))
            {
                throw DuplicateOptionException.Name(group.Name);
            }
        }

        foreach (var option in Options)
        {
            option.Validate();

            if (!names.Add(option.Name))
            {
                throw DuplicateOptionException.Name(option.Name);
            }

            var shortPrefix = option.ShortNamePrefix ?? ShortNamePrefixDefault;
            var longPrefix = option.LongNamePrefix ?? LongNamePrefixDefault;

            if (option.HasShortNames)
            {
                foreach (var shortName in option.ShortNames!)
                {
                    if (optionsDict.TryGetValue(shortPrefix + shortName, out var otherOption))
                    {
                        throw DuplicateOptionException.ShortOption(option.Name, otherOption.Name, shortName);
                    }

                    optionsDict[shortPrefix + shortName] = option;
                }

                shortOptionsList.Add(option);
                shortOptionPrefixTrie.Add(option.ShortNamePrefix ?? ShortNamePrefixDefault, option);
            }

            if (option.HasLongNames)
            {
                foreach (var longName in option.LongNames!)
                {
                    if (optionsDict.TryGetValue(longPrefix + longName, out var otherOption))
                    {
                        throw DuplicateOptionException.LongOption(option.Name, otherOption.Name, longName);
                    }

                    optionsDict[longPrefix + longName] = option;
                }

                longOptionsList.Add(option);
            }

            if (option.GroupNames != null)
            {
                foreach (var group in option.GroupNames)
                {
                    if (!optionGroups.ContainsKey(group))
                    {
                        throw new InvalidOptionException(option.Name, $"Option group {group} is not specified");
                    }
                }
            }
        }

        // value validation

        foreach (var value in Values)
        {
            value.Validate();

            if (!names.Add(value.Name))
            {
                throw DuplicateOptionException.Name(value.Name);
            }
        }

        var valuesRangeSorted = Values.ToList();
        valuesRangeSorted.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));

        for (var i = 0; i < valuesRangeSorted.Count - 1; i++)
        {
            var curEndIndex = valuesRangeSorted[i].EndIndex;
            if (!curEndIndex.HasValue || curEndIndex.Value >= valuesRangeSorted[i + 1].StartIndex)
            {
                throw new InvalidOptionException(valuesRangeSorted[i + 1].Name, $"The value range \"{valuesRangeSorted[i + 1].Name}\" overlaps with \"{valuesRangeSorted[i].Name}\"");
            }
        }

        // command validation

        var commandNames = new HashSet<string>();

        foreach (var command in Commands)
        {
            if (!commandNames.Add(command.Name))
            {
                throw DuplicateOptionException.CommandName(command.Name);
            }
        }

        foreach (var command in Commands)
        {
            if (command.Aliases == null)
            {
                continue;
            }

            foreach (var alias in command.Aliases)
            {
                if (!commandNames.Add(alias) && alias != command.Name)
                {
                    throw DuplicateOptionException.CommandName(alias);
                }
            }
        }
    }

    private void ParsePostValidation(List<ArgumentToken> argTokens)
    {
        var optionsGroups = new Dictionary<string, List<Option>>();
        var optionGroupTokens = new Dictionary<OptionGroup, HashSet<ArgumentToken>>();

        foreach (var group in OptionGroups)
        {
            optionsGroups[group.Name] = [];
            optionGroupTokens[group] = new HashSet<ArgumentToken>(new OptionGroupArgumentTokenComparer());
        }

        var optionCounts = new Dictionary<Option, int>();
        var requiredOptions = new List<Option>();

        foreach (var option in Options)
        {
            if (option.Required)
            {
                requiredOptions.Add(option);
            }

            if (option.GroupNames != null)
            {
                foreach (var group in option.GroupNames)
                {
                    optionsGroups[group].Add(option);
                }
            }
        }

        foreach (var arg in argTokens)
        {
            if (arg.Option == null)
            {
                continue;
            }

            optionCounts.TryGetValue(arg.Option, out var count);
            optionCounts[arg.Option] = count + 1;

            if (arg.Option.GroupNames != null)
            {
                foreach (var group in arg.Option.GroupNames)
                {
                    optionGroupTokens[OptionGroups.First(g => g.Name == group)].Add(arg);
                }
            }
        }

        foreach (var required in requiredOptions)
        {
            if (!optionCounts.ContainsKey(required))
            {
                throw OptionGroupOptionMismatchException.RequiredOptionMissing(required);
            }
        }

        foreach (var (option, count) in optionCounts)
        {
            if (option.Unique && count > 1)
            {
                ArgumentToken? first = null;
                ArgumentToken? second = null;

                for (var i = 0; i < argTokens.Count; i++)
                {
                    if (argTokens[i].Option == option)
                    {
                        if (first == null)
                        {
                            first = argTokens[i];
                        }
                        else
                        {
                            second = argTokens[i];
                        }
                    }
                }

                // first and second should not be null here
                throw new MultipleUniqueOptionException(first!, second!.ArgumentPosition);
            }
        }

        foreach (var (group, args) in optionGroupTokens)
        {
            if (!group.MatchesCount(args.Count, optionsGroups[group.Name].Count))
            {
                if (group.IsMutuallyExclusive)
                {
                    throw OptionGroupOptionMismatchException.MutuallyExclusiveOption(group, optionsGroups[group.Name], args);
                }
                else if (group.IsRequired)
                {
                    throw OptionGroupOptionMismatchException.RequiredOptionMissing(group, optionsGroups[group.Name]);
                }
                else
                {
                    throw OptionGroupOptionMismatchException.OptionMismatch(group, optionsGroups[group.Name], args);
                }
            }
        }
    }

    private void UseCommandContext(ICommand command)
    {
        Options = command.Options;
        OptionGroups = command.OptionGroups;
        Values = command.Values;
        Commands = command.Commands;
        ApplyOptions(command);
    }

    private bool ArgumentStartsWithDefaultOptionPrefixes(string arg)
    {
        return arg.StartsWith(LongNamePrefixDefault) || arg.StartsWith(ShortNamePrefixDefault);
    }

    private class OptionGroupArgumentTokenComparer : IEqualityComparer<ArgumentToken>
    {
        public bool Equals(ArgumentToken? x, ArgumentToken? y)
            => x?.Option?.Name == y?.Option?.Name;

        public int GetHashCode([DisallowNull] ArgumentToken obj)
            => obj.Option?.GetHashCode() ?? 0;
    }
}
