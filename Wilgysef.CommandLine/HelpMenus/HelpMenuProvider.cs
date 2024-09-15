using System.Collections;
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
    public HelpMenuProvider(ArgumentParser argParser, IReadOnlyList<ICommand>? commandList = null)
    {
        Parser = argParser;
        SetCommandList(commandList);
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
    /// Argument parser.
    /// </summary>
    protected ArgumentParser Parser { get; }

    /// <summary>
    /// The command list for which the help should be provided for.
    /// </summary>
    protected IReadOnlyList<ICommand>? CommandList { get; set; }

    /// <summary>
    /// Options to describe in help provider.
    /// </summary>
    protected ICollection<Option> Options { get; set; } = null!;

    /// <summary>
    /// Values to describe in help provider.
    /// </summary>
    protected ICollection<Value> Values { get; set; } = null!;

    /// <summary>
    /// Commands to describe in help provider.
    /// </summary>
    protected ICollection<ICommand> Commands { get; set; } = null!;

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
    /// Sets the command list for which the help should be provided for.
    /// </summary>
    /// <param name="commandList">Command list.</param>
    public virtual void SetCommandList(IReadOnlyList<ICommand>? commandList)
    {
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

            if (Values.Any(v => !v.PositionRange.Max.HasValue))
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
        var sortedValues = Values.OrderBy(v => v.PositionRange.Min);
        var lastPos = 0;

        foreach (var value in sortedValues)
        {
            for (; lastPos < value.PositionRange.Min; lastPos++)
            {
                yield return ValueUsagePlaceholder;
            }

            var name = GetValueName(value);

            if (value.PositionRange.Max.HasValue)
            {
                for (; lastPos <= value.PositionRange.Max; lastPos++)
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
            Values.OrderBy(v => v.PositionRange.Min),
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
    protected virtual string GetCommandName(ICommand command)
    {
        return command.Name;
    }

    /// <summary>
    /// Gets the command description.
    /// </summary>
    /// <param name="command">Command.</param>
    /// <returns>Command description.</returns>
    protected virtual string GetCommandDescription(ICommand command)
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
}
