using System.Text;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.HelpMenus;

/// <summary>
/// Command-line argument help menu provider.
/// </summary>
public class HelpMenuProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpMenuProvider"/> class.
    /// </summary>
    /// <param name="argParser">Argument parser.</param>
    /// <param name="commandList">The command list for which the help should be provided for.</param>
    public HelpMenuProvider(ArgumentParser argParser, IReadOnlyList<ICommandConfiguration>? commandList = null)
    {
        Parser = argParser;
        CommandList = commandList;

        if (CommandList != null && CommandList.Count > 0)
        {
            var lastCommand = CommandList[^1];
            Options = lastCommand.Options;
            Values = lastCommand.Values;
            Commands = lastCommand.Commands;
        }
        else
        {
            Options = Parser.Options;
            Values = Parser.Values;
            Commands = Parser.Commands;
        }
    }

    /// <summary>
    /// Application name.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Application description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indentation size of options, values, and commands.
    /// </summary>
    public int IndentSize { get; set; } = 4;

    /// <summary>
    /// Spacing between options/values/command names and their descriptions.
    /// </summary>
    public int TableColumnSpacing { get; set; } = 6;

    /// <summary>
    /// Maximum row length.
    /// </summary>
    public int MaxRowLength { get; set; } = 100;

    /// <summary>
    /// Description section header.
    /// </summary>
    public string? DescriptionHeader { get; set; } = "Description:";

    /// <summary>
    /// Usage section header.
    /// </summary>
    public string? UsageHeader { get; set; } = "Usage:";

    /// <summary>
    /// Usage command placeholder.
    /// </summary>
    public string UsageCommandPlaceholder { get; set; } = "[command]";

    /// <summary>
    /// Usage option placeholder.
    /// </summary>
    public string UsageOptionPlaceholder { get; set; } = "[options]";

    /// <summary>
    /// Values section header.
    /// </summary>
    public string? ValuesHeader { get; set; } = "Values:";

    /// <summary>
    /// Placeholder text for unspecified values in Usage section.
    /// </summary>
    public string ValueUsagePlaceholder { get; set; } = "value";

    /// <summary>
    /// Prefix for value names.
    /// </summary>
    public string ValueNamePrefix { get; set; } = "<";

    /// <summary>
    /// Suffix for value names.
    /// </summary>
    public string ValueNameSuffix { get; set; } = ">";

    /// <summary>
    /// Commands section header.
    /// </summary>
    public string? CommandsHeader { get; set; } = "Commands:";

    /// <summary>
    /// Options section header.
    /// </summary>
    public string? OptionsHeader { get; set; } = "Options:";

    /// <summary>
    /// Option names separator.
    /// </summary>
    public string OptionNamesSeparator { get; set; } = ", ";

    /// <summary>
    /// Prefix for option value names.
    /// </summary>
    public string OptionValuePrefix { get; set; } = "<";

    /// <summary>
    /// Suffix for option value names.
    /// </summary>
    public string OptionValueSuffix { get; set; } = ">";

    /// <summary>
    /// Prefix for optional option value names.
    /// </summary>
    public string OptionValueOptionalPrefix { get; set; } = "[";

    /// <summary>
    /// Suffix for optional option value names.
    /// </summary>
    public string OptionValueOptionalSuffix { get; set; } = "]";

    /// <summary>
    /// Indicator that the option takes more values than displayed.
    /// </summary>
    public string ValueContinued { get; set; } = "...";

    /// <summary>
    /// The similarity distance to use when displaying similar options.
    /// </summary>
    public int SimilarValueDistance { get; set; } = 3;

    /// <summary>
    /// Argument parser.
    /// </summary>
    protected ArgumentParser Parser { get; }

    /// <summary>
    /// The command list for which the help should be provided for.
    /// </summary>
    protected IReadOnlyList<ICommandConfiguration>? CommandList { get; }

    /// <summary>
    /// Options to describe in help provider.
    /// </summary>
    protected ICollection<Option> Options { get; }

    /// <summary>
    /// Values to describe in help provider.
    /// </summary>
    protected ICollection<Value> Values { get; }

    /// <summary>
    /// Commands to describe in help provider.
    /// </summary>
    protected ICollection<ICommandConfiguration> Commands { get; }

    /// <summary>
    /// Gets the help menu.
    /// </summary>
    /// <returns>Help menu lines.</returns>
    public virtual IEnumerable<string> GetHelpMenu()
    {
        foreach (var section in GetSections())
        {
            var hasContent = false;
            foreach (var line in section)
            {
                hasContent = true;
                yield return line;
            }

            if (hasContent)
            {
                yield return "";
            }
        }
    }

    /// <summary>
    /// Gets the unknown command message.
    /// </summary>
    /// <param name="value">Unknown command.</param>
    /// <param name="expectedCommands">Expected commands.</param>
    /// <returns>Unknown command message lines.</returns>
    public virtual IEnumerable<string> GetUnknownCommandMessage(string value, IEnumerable<ICommandConfiguration> expectedCommands)
    {
        yield return $"Error: \"{value}\" is not a known command.";

        var similarCommands = GetSimilarCommands(value, expectedCommands).ToList();

        if (similarCommands.Count > 0)
        {
            yield return "";
            yield return "The most similar commands are:";

            foreach (var similar in similarCommands)
            {
                yield return $"        {similar.Name}";
            }
        }
    }

    /// <summary>
    /// Gets the unknown option message.
    /// </summary>
    /// <param name="value">Unknown option.</param>
    /// <param name="expectedOptions">Expected options.</param>
    /// <returns>Unknown option message lines.</returns>
    public virtual IEnumerable<string> GetUnknownOptionMessage(string value, IEnumerable<Option> expectedOptions)
    {
        yield return $"Error: \"{value}\" is not a known option.";

        var similarOptions = GetSimilarOptions(value, expectedOptions).ToList();

        if (similarOptions.Count > 0)
        {
            yield return "";
            yield return "The most similar options are:";

            foreach (var similar in similarOptions)
            {
                yield return $"        {similar.GetOptionArgument(Parser.ShortNamePrefixDefault, Parser.LongNamePrefixDefault)}";
            }
        }
    }

    /// <summary>
    /// Gets the order of sections to display.
    /// </summary>
    /// <returns>Sections to display.</returns>
    protected virtual IEnumerable<IEnumerable<string>> GetSections()
    {
        yield return GetDescription();
        yield return GetUsage();
        yield return GetValues();
        yield return GetCommands();
        yield return GetOptions();
    }

    /// <summary>
    /// Gets the description section.
    /// </summary>
    /// <returns>Description section.</returns>
    protected virtual IEnumerable<string> GetDescription()
    {
        if (Description != null)
        {
            if (DescriptionHeader != null)
            {
                yield return DescriptionHeader;
            }

            foreach (var line in Wrap($"{new string(' ', IndentSize)}{Description}", IndentSize))
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Gets the usage section.
    /// </summary>
    /// <returns>Usage section.</returns>
    protected virtual IEnumerable<string> GetUsage()
    {
        if (UsageHeader != null)
        {
            yield return UsageHeader;
        }

        var builder = new StringBuilder();
        builder.Append(' ', IndentSize)
            .Append(ApplicationName ?? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));

        if (CommandList != null && CommandList.Count > 0)
        {
            foreach (var command in CommandList)
            {
                builder.Append(' ')
                    .Append(command.Name);
            }
        }

        if (Values.Count > 0)
        {
            foreach (var value in GetValuesValueNames())
            {
                builder.Append(' ')
                    .Append(ValueNamePrefix)
                    .Append(value)
                    .Append(ValueNameSuffix);
            }

            if (Values.Any(v => !v.EndIndex.HasValue))
            {
                builder.Append(' ')
                    .Append(ValueContinued);
            }
        }

        if (Options.Count > 0)
        {
            builder.Append(' ')
                .Append(UsageOptionPlaceholder);
        }

        if (Commands.Count > 0)
        {
            builder.Append(' ')
                .Append(UsageCommandPlaceholder);
        }

        foreach (var line in Wrap(builder.ToString(), IndentSize))
        {
            yield return line;
        }
    }

    #region Values

    /// <summary>
    /// Gets the <see cref="Values"/> names.
    /// </summary>
    /// <returns>Value names.</returns>
    protected virtual IEnumerable<string> GetValuesValueNames()
    {
        var sortedValues = Values.OrderBy(v => v.StartIndex);
        var lastPos = 0;

        foreach (var value in sortedValues)
        {
            for (; lastPos < value.StartIndex; lastPos++)
            {
                yield return ValueUsagePlaceholder;
            }

            var name = GetValueName(value);

            if (value.EndIndex.HasValue)
            {
                for (; lastPos <= value.EndIndex.Value; lastPos++)
                {
                    yield return name;
                }
            }
            else
            {
                yield return name;
                break;
            }
        }
    }

    /// <summary>
    /// Gets the values section.
    /// </summary>
    /// <returns>Values section.</returns>
    protected virtual IEnumerable<string> GetValues()
    {
        return GetTwoColumnTable(
            Values.OrderBy(v => v.StartIndex),
            ValuesHeader,
            GetValueName,
            GetValueDescription);
    }

    /// <summary>
    /// Gets the value name.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Value name.</returns>
    protected virtual string GetValueName(Value value)
    {
        return value.ValueName ?? value.Name;
    }

    /// <summary>
    /// Gets the value description.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Value description.</returns>
    protected virtual string GetValueDescription(Value value)
    {
        return value.Description ?? "";
    }

    #endregion

    #region Commands

    /// <summary>
    /// Gets the commands section.
    /// </summary>
    /// <returns>Commands section.</returns>
    protected virtual IEnumerable<string> GetCommands()
    {
        return GetTwoColumnTable(
            Commands.Where(c => !c.Hidden).ToList(),
            CommandsHeader,
            GetCommandName,
            GetCommandDescription);
    }

    /// <summary>
    /// Gets the command name.
    /// </summary>
    /// <param name="command">Command.</param>
    /// <returns>Command name.</returns>
    protected virtual string GetCommandName(ICommandConfiguration command)
    {
        return command.Name;
    }

    /// <summary>
    /// Gets the command description.
    /// </summary>
    /// <param name="command">Command.</param>
    /// <returns>Command description.</returns>
    protected virtual string GetCommandDescription(ICommandConfiguration command)
    {
        return command.Description ?? "";
    }

    #endregion

    #region Options

    /// <summary>
    /// Gets the options section.
    /// </summary>
    /// <returns>Options section.</returns>
    protected virtual IEnumerable<string> GetOptions()
    {
        return GetTwoColumnTable(
            Options.Where(o => !o.Hidden).ToList(),
            OptionsHeader,
            GetOptionArgumentAndValues,
            GetOptionDescription);
    }

    /// <summary>
    /// Gets the option argument and values.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Option argument and values.</returns>
    protected virtual string GetOptionArgumentAndValues(Option option)
    {
        var builder = new StringBuilder();
        builder.Append(string.Join(OptionNamesSeparator, GetOptionNames(option)));

        var valueNames = GetOptionValues(option);
        if (valueNames.Any())
        {
            builder.Append(' ')
                .Append(string.Join(" ", valueNames));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Gets the option argument names.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Option argument names.</returns>
    protected virtual IEnumerable<string> GetOptionNames(Option option)
    {
        return GetOptionShortArguments(option)
            .Concat(GetOptionLongArguments(option));
    }

    /// <summary>
    /// Gets the option short arguments.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Short arguments.</returns>
    protected virtual IEnumerable<string> GetOptionShortArguments(Option option)
    {
        if (option.HasShortNames)
        {
            var prefix = option.ShortNamePrefix ?? Parser.ShortNamePrefixDefault;
            foreach (var ch in option.ShortNames!)
            {
                yield return prefix + ch.ToString();
            }
        }
    }

    /// <summary>
    /// Gets the option long arguments.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Long arguments.</returns>
    protected virtual IEnumerable<string> GetOptionLongArguments(Option option)
    {
        if (option.HasLongNames)
        {
            var prefix = option.LongNamePrefix ?? Parser.LongNamePrefixDefault;
            foreach (var name in option.LongNames!)
            {
                if (option.SwitchNegateLongPrefix != null)
                {
                    yield return $"{prefix}[{option.SwitchNegateLongPrefix}]{name}";
                }
                else
                {
                    yield return prefix + name;
                }
            }
        }
    }

    /// <summary>
    /// Gets the option value names.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Value names.</returns>
    protected virtual IEnumerable<string> GetOptionValues(Option option)
    {
        if (option.ValueCountRange == null || option.ValueNames == null)
        {
            yield break;
        }

        var maxValues = option.ValueCountRange.Max ?? option.ValueNames.Count;
        var enumerator = option.ValueNames.GetEnumerator();
        var name = option.Name;

        for (var i = 0; i < maxValues; i++)
        {
            if (enumerator.MoveNext())
            {
                name = enumerator.Current;
            }

            if (i < option.ValueCountRange.Min)
            {
                yield return $"{OptionValuePrefix}{name}{OptionValueSuffix}";
            }
            else
            {
                yield return $"{OptionValueOptionalPrefix}{name}{OptionValueOptionalSuffix}";
            }
        }

        if (!option.ValueCountRange.Max.HasValue && ValueContinued.Length > 0)
        {
            yield return ValueContinued;
        }
    }

    /// <summary>
    /// Gets the option description.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns>Option description.</returns>
    protected virtual string GetOptionDescription(Option option)
    {
        return option.Description ?? "";
    }

    #endregion

    /// <summary>
    /// Gets an aligned two-column table.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="items">Items.</param>
    /// <param name="header">Section header.</param>
    /// <param name="leftColumnFactory">Left column factory.</param>
    /// <param name="rightColumnFactory">Right column factory.</param>
    /// <returns>Table.</returns>
    protected virtual IEnumerable<string> GetTwoColumnTable<T>(
        IEnumerable<T> items,
        string? header,
        Func<T, string> leftColumnFactory,
        Func<T, string> rightColumnFactory)
    {
        if (!items.Any())
        {
            yield break;
        }

        if (header != null)
        {
            yield return header;
        }

        var leftColumnLength = 0;
        var lineColumns = new List<(string LeftColumn, string RightColumn, bool Wrap)>();

        foreach (var item in items)
        {
            var leftColumn = leftColumnFactory(item);
            var rightColumn = rightColumnFactory(item);
            var wrap = false;

            if (IndentSize + leftColumn.Length + TableColumnSpacing + rightColumn.Length < MaxRowLength)
            {
                // only set left-column length if it does not overflow
                if (leftColumn.Length > leftColumnLength)
                {
                    leftColumnLength = leftColumn.Length;
                }
            }
            else
            {
                wrap = true;
            }

            lineColumns.Add((leftColumn, rightColumn, wrap));
        }

        if (leftColumnLength == 0)
        {
            // all lines are wrapping
            leftColumnLength = IndentSize - TableColumnSpacing;
        }

        var spacing = new string(' ', TableColumnSpacing);
        var indent = new string(' ', IndentSize);
        var wrapPad = new string(' ', IndentSize + leftColumnLength + TableColumnSpacing);

        foreach (var (leftColumn, rightColumn, wrap) in lineColumns)
        {
            if (!wrap)
            {
                yield return $"{indent}{leftColumn.PadRight(leftColumnLength)}{spacing}{rightColumn}";
            }
            else
            {
                yield return $"{indent}{leftColumn}";

                foreach (var line in Wrap($"{wrapPad}{rightColumn}", wrapPad.Length))
                {
                    yield return line;
                }
            }
        }
    }

    /// <summary>
    /// Wraps a line into multiple if it exceeds <see cref="MaxRowLength"/>.
    /// </summary>
    /// <param name="line">Line.</param>
    /// <param name="indentSize">Indent size for wrapped lines.</param>
    /// <returns>Wrapped lines.</returns>
    protected virtual IEnumerable<string> Wrap(string line, int indentSize)
    {
        if (line.Length <= MaxRowLength)
        {
            yield return line;
            yield break;
        }

        var builder = new StringBuilder();
        var words = line.Split(' ');
        var indent = new string(' ', indentSize);

        builder.Append(words[0]);

        for (var i = 1; i < words.Length; i++)
        {
            if (builder.Length + 1 + words[i].Length > MaxRowLength)
            {
                yield return builder.ToString();

                builder.Clear()
                    .Append(indent)
                    .Append(words[i]);
            }
            else
            {
                builder.Append(' ')
                    .Append(words[i]);
            }
        }

        yield return builder.ToString();
    }

    /// <summary>
    /// Gets a list of similar commands to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="expectedCommands">Expected commands.</param>
    /// <returns>Similar commands.</returns>
    protected virtual IEnumerable<ICommandConfiguration> GetSimilarCommands(string value, IEnumerable<ICommandConfiguration> expectedCommands)
    {
        var threshold = 3;

        var similar = new List<(ICommandConfiguration Command, int Distance)>();

        foreach (var command in expectedCommands)
        {
            var distance = DamerauLevenshteinDistance(value, command.Name, threshold, command.CaseInsensitiveNameMatch);
            if (distance <= threshold)
            {
                similar.Add((command, distance));
            }
        }

        return similar.OrderBy(e => e.Distance)
            .Select(e => e.Command);
    }

    /// <summary>
    /// Gets a list of similar commands to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="expectedOptions">Expected options.</param>
    /// <returns>Similar commands.</returns>
    protected virtual IEnumerable<Option> GetSimilarOptions(string value, IEnumerable<Option> expectedOptions)
    {
        var similar = new List<(Option Option, int Distance)>();

        foreach (var option in expectedOptions)
        {
            var distance = GetOptionDistance(option, value);
            if (distance <= SimilarValueDistance)
            {
                similar.Add((option, distance));
            }
        }

        return similar.OrderBy(e => e.Distance)
            .Select(e => e.Option);

        int GetOptionDistance(Option option, string value)
        {
            var minDistance = int.MaxValue;

            var keyValueSeparators = option.KeyValueSeparators ?? Parser.KeyValueSeparatorsDefault;
            var ignoreCase = option.LongNameCaseInsensitive ?? Parser.LongNameCaseInsensitiveDefault;

            if (keyValueSeparators.Count == 0)
            {
                GetOptionDistanceInternal(value);
                return minDistance;
            }

            foreach (var sep in keyValueSeparators)
            {
                var sepIdx = value.IndexOf(sep);
                GetOptionDistanceInternal(sepIdx != -1 ? value[..sepIdx] : value);
            }

            return minDistance;

            void GetOptionDistanceInternal(string value)
            {
                if (option.LongNames != null)
                {
                    var longPrefix = option.LongNamePrefix ?? Parser.LongNamePrefixDefault;
                    foreach (var longName in option.LongNames)
                    {
                        UpdateDistance(value, $"{longPrefix}{longName}");
                    }

                    if (option.Switch && option.SwitchNegateLongPrefix != null)
                    {
                        foreach (var longName in option.LongNames)
                        {
                            UpdateDistance(value, $"{longPrefix}{option.SwitchNegateLongPrefix}{longName}");
                        }
                    }
                }

                void UpdateDistance(string s, string t)
                {
                    var distance = DamerauLevenshteinDistance(s, t, SimilarValueDistance, ignoreCase);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }
        }
    }

    // https://stackoverflow.com/a/9454016
    private static int DamerauLevenshteinDistance(
        string source,
        string target,
        int threshold,
        bool ignoreCase)
    {
        int length1 = source.Length;
        int length2 = target.Length;

        if (Math.Abs(length1 - length2) > threshold)
        {
            return int.MaxValue;
        }

        // ensure arrays [i] / length1 use shorter length
        if (length1 > length2)
        {
            (target, source) = (source, target);
            (length2, length1) = (length1, length2);
        }

        int maxi = length1;
        int maxj = length2;

        int[] dCurrent = new int[maxi + 1];
        int[] dMinus1 = new int[maxi + 1];
        int[] dMinus2 = new int[maxi + 1];
        int[] dSwap;

        for (int i = 0; i <= maxi; i++)
        {
            dCurrent[i] = i;
        }

        int jm1 = 0;
        int im1;
        int im2;

        for (int j = 1; j <= maxj; j++)
        {
            dSwap = dMinus2;
            dMinus2 = dMinus1;
            dMinus1 = dCurrent;
            dCurrent = dSwap;

            int minDistance = int.MaxValue;
            dCurrent[0] = j;
            im1 = 0;
            im2 = -1;

            for (int i = 1; i <= maxi; i++)
            {
                int cost = Equal(source[im1], target[jm1]) ? 0 : 1;

                int del = dCurrent[im1] + 1;
                int ins = dMinus1[i] + 1;
                int sub = dMinus1[im1] + cost;

                int min = (del > ins)
                    ? (ins > sub ? sub : ins)
                    : (del > sub ? sub : del);

                if (i > 1
                    && j > 1
                    && Equal(source[im2], target[jm1])
                    && Equal(source[im1], target[j - 2]))
                {
                    min = Math.Min(min, dMinus2[im2] + cost);
                }

                dCurrent[i] = min;
                if (min < minDistance)
                {
                    minDistance = min;
                }

                im1++;
                im2++;
            }

            jm1++;
            if (minDistance > threshold)
            {
                return int.MaxValue;
            }
        }

        int result = dCurrent[maxi];
        return (result > threshold) ? int.MaxValue : result;

        bool Equal(char a, char b)
            => ignoreCase
                ? char.ToUpperInvariant(a) == char.ToUpperInvariant(b)
                : a == b;
    }
}
