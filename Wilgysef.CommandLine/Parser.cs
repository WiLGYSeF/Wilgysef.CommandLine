using System.Text.RegularExpressions;

namespace Wilgysef.CommandLine;

public class Parser
{
    public IList<Option> Options { get; set; }

    public string ShortNameCharsDefault { get; set; } = "-";

    public string LongNameCharsDefault { get; set; } = "--";

    public string KeyValueSeparatorDefault { get; set; } = "=";

    public string ArgumentLiteralSeparator { get; set; } = "--";

    public ParsedResult Parse(IEnumerable<string> args)
    {
        ValidateOptions();

        var longOptions = new List<Option>();
        var shortOptions = new List<Option>();
        var shortOptionPrefixTrie = new Trie<Option>();

        foreach (var option in Options)
        {
            if (option.HasLongNames)
            {
                longOptions.Add(option);
            }

            if (option.HasShortNames)
            {
                shortOptions.Add(option);
                shortOptionPrefixTrie.Add(option.ShortNamePrefix ?? ShortNameCharsDefault, option);
            }
        }

        var argsEnumerator = new BufferedEnumerator<string>(args.GetEnumerator(), 1);
        var parsedArgs = new List<ParsedArgument>();
        var position = 1;
        var parseLiteral = false;

        while (argsEnumerator.MoveNext())
        {
            var arg = argsEnumerator.Current;
            if (arg == ArgumentLiteralSeparator)
            {
                parseLiteral = true;
                continue;
            }

            var matched = !parseLiteral && ParseArg(arg, parsedArgs);
            if (!matched)
            {
                parsedArgs.Add(ParsedArgument.Unparsed(arg));
            }

            position++;
        }

        return new ParsedResult(parsedArgs);

        bool ParseArg(string arg, List<ParsedArgument> parsedArgs)
        {
            foreach (var option in longOptions)
            {
                var parsedArg = ParseLongOption(option, arg, argsEnumerator);
                if (parsedArg != null)
                {
                    parsedArgs.Add(parsedArg);
                    return true;
                }
            }

            var options = shortOptionPrefixTrie.GetValues(arg, out var triePrefix);
            if (options != null)
            {
                var parsedShortArgs = ParseShortOption(options, arg, arg[..triePrefix!.Value], argsEnumerator);
                if (parsedShortArgs != null)
                {
                    parsedArgs.AddRange(parsedShortArgs);
                    return true;
                }
            }

            return false;
        }
    }

    private ParsedArgument? ParseLongOption(
        Option option,
        string arg,
        BufferedEnumerator<string> argsEnumerator)
    {
        if (!MatchesLongOption(option, arg, out var value))
        {
            return null;
        }

        if (!option.CanHaveValues)
        {
            // TODO: throw if it shouldn't have a value when it does
            return ParsedArgument.NoValue(option.Name);
        }

        var values = new List<string>();

        var currentArgs = 0;
        if (value != null)
        {
            values.Add(value);
            currentArgs++;
        }

        while (option.ValueCountRange!.UnderMax(currentArgs) && argsEnumerator.MoveNext())
        {
            foreach (var otherOpt in Options)
            {
                if (MatchesOption(otherOpt, argsEnumerator.Current, out var _))
                {
                    // TODO: how to escape match?
                    argsEnumerator.Rollback(1);
                    return ParsedArgument.WithValues(option.Name, values);
                }
            }

            values.Add(argsEnumerator.Current);
            currentArgs++;
        }

        // TODO: throw if values do not match range

        return ParsedArgument.WithValues(option.Name, values);
    }

    private List<ParsedArgument>? ParseShortOption(
        IEnumerable<Option> options,
        string arg,
        string prefix,
        BufferedEnumerator<string> argsEnumerator)
    {
        var parsedArguments = new List<ParsedArgument>();

        for (var i = prefix.Length; i < arg.Length; i++)
        {
            var option = MatchOption(arg[i]);
            if (option == null)
            {
                // TODO: unknown short arg
                throw new Exception();
            }

            var keyValueSeparator = option.KeyValueSeparator ?? KeyValueSeparatorDefault;
            var keyValue = arg.AsSpan(i + 1).StartsWith(keyValueSeparator)
                ? arg[(i + 1 + keyValueSeparator.Length)..]
                : null;
            if (!option.CanHaveValues)
            {
                if (keyValue != null)
                {
                    // TODO: throw if it shouldn't have a value when it does
                }

                parsedArguments.Add(ParsedArgument.NoValue(option.Name));
                continue;
            }

            var values = new List<string>();
            var currentArgs = 0;

            if (keyValue != null)
            {
                values.Add(keyValue);
                currentArgs++;
            }
            else if (i < arg.Length - 1)
            {
                values.Add(arg[(i + 1)..]);
                currentArgs++;
            }

            while (option.ValueCountRange!.UnderMax(currentArgs) && argsEnumerator.MoveNext())
            {
                foreach (var otherOpt in Options)
                {
                    if (MatchesOption(otherOpt, argsEnumerator.Current, out var _))
                    {
                        // TODO: how to escape match?
                        argsEnumerator.Rollback(1);
                        parsedArguments.Add(ParsedArgument.WithValues(option.Name, values));
                        return parsedArguments;
                    }
                }

                values.Add(argsEnumerator.Current);
                currentArgs++;
            }

            // TODO: throw if values do not match range

            parsedArguments.Add(ParsedArgument.WithValues(option.Name, values));
            break;
        }

        return parsedArguments;

        Option? MatchOption(char ch)
        {
            foreach (var option in options)
            {
                if (option.MatchesShortName(ch))
                {
                    return option;
                }
            }

            return null;
        }
    }

    private bool MatchesOption(Option option, string arg, out string? value)
    {
        return MatchesLongOption(option, arg, out value)
            || MatchesShortOption(option, arg, out value);
    }

    private bool MatchesLongOption(Option option, string arg, out string? value)
    {
        var longPrefix = option.LongNamePrefix ?? LongNameCharsDefault;

        if (!option.HasLongNames || !arg.StartsWith(longPrefix))
        {
            value = null;
            return false;
        }

        var keyValueSep = option.KeyValueSeparator ?? KeyValueSeparatorDefault;
        var keyValueSepIdx = arg.IndexOf(keyValueSep);
        if (keyValueSepIdx == -1)
        {
            keyValueSepIdx = arg.Length;
        }

        var argSubstring = arg[longPrefix.Length..keyValueSepIdx];
        if (argSubstring.Length == 0)
        {
            value = null;
            return false;
        }

        var comparison = option.LongNameCaseInsensitive
            ? StringComparison.CurrentCultureIgnoreCase
            : StringComparison.CurrentCulture;

        value = keyValueSepIdx < arg.Length
            ? arg[(keyValueSepIdx + keyValueSep.Length)..]
            : null;
        return option.LongNames!.Any(n => n.Equals(argSubstring, comparison));
    }

    private bool MatchesShortOption(Option option, string arg, out string? value)
    {
        var shortPrefix = option.ShortNamePrefix ?? ShortNameCharsDefault;
        if (!option.HasShortNames || !arg.StartsWith(shortPrefix))
        {
            value = null;
            return false;
        }

        var keyValueSep = option.KeyValueSeparator ?? KeyValueSeparatorDefault;
        var keyValueSepIdx = arg.IndexOf(keyValueSep);
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
            && !arg.AsSpan(shortPrefix.Length + matchIdx + 1).StartsWith(keyValueSep))
        {
            value = matchIdx + 1 < argSubstring.Length
                ? argSubstring[(matchIdx + 1)..]
                : null;
        }
        else
        {
            value = keyValueSepIdx < arg.Length
                ? arg[(keyValueSepIdx + keyValueSep.Length)..]
                : null;
        }

        return true;
    }

    private void ValidateOptions()
    {
        foreach (var option in Options)
        {
            option.Validate();
        }

        // TODO: check duplicate names and defaults
    }
}
