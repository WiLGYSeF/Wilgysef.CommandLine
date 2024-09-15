using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Used to parse argument options.
/// </summary>
public class Option : IOptionProperties
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Option"/> class.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
    public Option(string name)
    {
        if (name.Length == 0)
        {
            throw new ArgumentException("Name length must be greater than 0", nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Option name.
    /// </summary>
    /// <remarks>
    /// Used to determine the instance property to set when parsing.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Option description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the option should be hidden from help providers.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Value names to display in help providers.
    /// </summary>
    public ICollection<string>? ValueNames { get; set; }

    /// <summary>
    /// Option short names.
    /// </summary>
    public ICollection<char>? ShortNames { get; set; }

    /// <summary>
    /// Option long names.
    /// </summary>
    public ICollection<string>? LongNames { get; set; }

    /// <inheritdoc/>
    public string? ShortNamePrefix { get; set; }

    /// <inheritdoc/>
    public string? LongNamePrefix { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? KeyValueSeparators { get; set; }

    /// <summary>
    /// Number of values the option may have.
    /// </summary>
    public ValueRange? ValueCountRange { get; set; }

    /// <summary>
    /// Indicates if the option is a boolean switch value.
    /// </summary>
    public bool Switch { get; set; }

    /// <summary>
    /// Switch negation long prefix used to configure a negatable option.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--no-cache</c>, <c>"no-"</c> is the negated switch long prefix.
    /// </remarks>
    public string? SwitchNegateLongPrefix { get; set; }

    /// <summary>
    /// Switch negation short names.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P</c>, <c>P</c> is the negated short name.
    /// </remarks>
    public ICollection<char>? SwitchNegateShortNames { get; set; }

    /// <summary>
    /// Indicates if the option is a counter value.
    /// </summary>
    public bool Counter { get; set; }

    /// <summary>
    /// Indicates if the option is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Name of groups the option belongs to.
    /// </summary>
    public ICollection<string>? GroupNames { get; set; }

    /// <summary>
    /// Indicates if the option should only be provided once.
    /// </summary>
    public bool Unique { get; set; }

    /// <inheritdoc/>
    public bool? ShortNameImmediateValue { get; set; }

    /// <inheritdoc/>
    public bool? LongNameCaseInsensitive { get; set; }

    /// <inheritdoc/>
    public bool? KeepFirstValue { get; set; }

    /// <summary>
    /// Indicates if the option has short names.
    /// </summary>
    public bool HasShortNames => ShortNames != null && ShortNames.Count > 0;

    /// <summary>
    /// Indicates if the option has long names.
    /// </summary>
    public bool HasLongNames => LongNames != null && LongNames.Count > 0;

    /// <summary>
    /// Indicates if the option can have values.
    /// </summary>
    public bool CanHaveValues => ValueCountRange != null
        && (!ValueCountRange.Max.HasValue || ValueCountRange.Max > 0);

    #region Static initializers

    /// <summary>
    /// Creates an option with a short name that takes one value.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionWithValue(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            ValueCountRange = new ValueRange(1, 1),
        };
    }

    /// <summary>
    /// Creates an option with a short name that optionally takes one value.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionWithOptionalValue(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            ValueCountRange = new ValueRange(0, 1),
        };
    }

    /// <summary>
    /// Creates an option with a short name that takes multiple values.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <param name="values">Value count.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionWithValues(string name, char opt, int values)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            ValueCountRange = new ValueRange(values, values),
        };
    }

    /// <summary>
    /// Creates an option with a short name that takes a range of values.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <param name="min">Minimum value count.</param>
    /// <param name="max">Maximum value count.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionWithValueRange(string name, char opt, int min, int? max)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            ValueCountRange = new ValueRange(min, max),
        };
    }

    /// <summary>
    /// Creates a switch option with a short name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionSwitch(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            Switch = true,
        };
    }

    /// <summary>
    /// Creates a negatable switch option with a short name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <param name="negateOpt">Negated short name.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionNegatableSwitch(string name, char opt, char negateOpt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            Switch = true,
            SwitchNegateShortNames = [negateOpt],
        };
    }

    /// <summary>
    /// Creates a counter option with a short name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Short name.</param>
    /// <returns>Option.</returns>
    public static Option ShortOptionCounter(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            Counter = true,
        };
    }

    /// <summary>
    /// Creates an option with a long name that takes one value.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionWithValue(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
            ValueCountRange = new ValueRange(1, 1),
        };
    }

    /// <summary>
    /// Creates an option with a long name that optionally takes one value.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionWithOptionalValue(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
            ValueCountRange = new ValueRange(0, 1),
        };
    }

    /// <summary>
    /// Creates an option with a long name that takes multiple values.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <param name="values">Value count.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionWithValues(string name, string opt, int values)
    {
        return new Option(name)
        {
            LongNames = [opt],
            ValueCountRange = new ValueRange(values, values),
        };
    }

    /// <summary>
    /// Creates an option with a long name that takes a range of values.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <param name="min">Minimum value count.</param>
    /// <param name="max">Maximum value count.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionWithValueRange(string name, string opt, int min, int? max)
    {
        return new Option(name)
        {
            LongNames = [opt],
            ValueCountRange = new ValueRange(min, max),
        };
    }

    /// <summary>
    /// Creates a switch option with a long name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionSwitch(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
            Switch = true,
        };
    }

    /// <summary>
    /// Creates a negatable switch option with a long name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <param name="prefix">Negation prefix.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionNegatableSwitch(string name, string opt, string? prefix = "no-")
    {
        return new Option(name)
        {
            LongNames = [opt],
            Switch = true,
            SwitchNegateLongPrefix = prefix,
        };
    }

    /// <summary>
    /// Creates a counter option with a long name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <param name="opt">Long name.</param>
    /// <returns>Option.</returns>
    public static Option LongOptionCounter(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
            Counter = true,
        };
    }

    #endregion

    /// <summary>
    /// Checks if <paramref name="ch"/> matches the option.
    /// </summary>
    /// <param name="ch">Argument name, without prefixes or key-values.</param>
    /// <param name="switchNegated">Indicates if the argument is a switch-negation.</param>
    /// <returns><see langword="true"/> if <paramref name="ch"/> matches the option, otherwise <see langword="false"/>.</returns>
    public bool MatchesShortName(char ch, out bool switchNegated)
    {
        if (ShortNames == null)
        {
            switchNegated = false;
            return false;
        }

        if (Switch && SwitchNegateShortNames != null && SwitchNegateShortNames.Contains(ch))
        {
            switchNegated = true;
            return true;
        }

        switchNegated = false;
        return ShortNames.Contains(ch);
    }

    /// <summary>
    /// Checks if <paramref name="argName"/> matches the option.
    /// </summary>
    /// <param name="argName">Argument name, without prefixes or key-values.</param>
    /// <param name="switchNegated">Indicates if the argument is a switch-negation.</param>
    /// <param name="ignoreCaseDefault">Whether to ignore case-sensitivity if <see cref="LongNameCaseInsensitive"/> is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="argName"/> matches the option, otherwise <see langword="false"/>.</returns>
    public bool MatchesLongName(string argName, out bool switchNegated, bool ignoreCaseDefault = false)
    {
        if (LongNames == null)
        {
            switchNegated = false;
            return false;
        }

        var comparison = (LongNameCaseInsensitive ?? ignoreCaseDefault)
            ? StringComparison.CurrentCultureIgnoreCase
            : StringComparison.CurrentCulture;

        if (Switch && SwitchNegateLongPrefix != null)
        {
            if (LongNames.Any(n => (SwitchNegateLongPrefix + n).Equals(argName, comparison)))
            {
                switchNegated = true;
                return true;
            }
        }

        switchNegated = false;
        return LongNames.Any(n => n.Equals(argName, comparison));
    }

    /// <summary>
    /// Gets the option argument.
    /// </summary>
    /// <param name="shortNamePrefixDefault">Short name prefix default if <see cref="ShortNamePrefix"/> is <see langword="null"/>.</param>
    /// <param name="longNamePrefixDefault">Long name prefix default if <see cref="LongNamePrefix"/> is <see langword="null"/>.</param>
    /// <returns>Option argument.</returns>
    public string GetOptionArgument(string shortNamePrefixDefault = "-", string longNamePrefixDefault = "--")
    {
        return LongNames != null
            ? $"{LongNamePrefix ?? longNamePrefixDefault}{LongNames.First()}"
            : $"{ShortNamePrefix ?? shortNamePrefixDefault}{ShortNames!.First()}";
    }

    /// <summary>
    /// Validates the option.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the option is invalid.</exception>
    public void Validate()
    {
        ThrowIf(!HasShortNames && !HasLongNames, "Option does not have long or short names set");
        ThrowIf(HasLongNames && LongNames!.Any(n => n.Length == 0), "Long names must have a length greater than 0");

        ThrowIf(ShortNamePrefix != null && ShortNamePrefix.Length == 0, "Short name prefix cannot be empty");
        ThrowIf(LongNamePrefix != null && LongNamePrefix.Length == 0, "Long name prefix cannot be empty");

        ThrowIf(KeyValueSeparators != null && KeyValueSeparators.Any(string.IsNullOrEmpty), "Key-value separator cannot be empty");

        ThrowIf((ValueCountRange != null ? 1 : 0) + (Switch ? 1 : 0) + (Counter ? 1 : 0) > 1, "Option can either take values, be a switch, or counter, only at one time");

        ThrowIf(ValueCountRange != null && ValueCountRange.Max.HasValue && ValueCountRange.Min > ValueCountRange.Max, "Minimum value range cannot be greater than maximum");

        ThrowIf(SwitchNegateLongPrefix != null && SwitchNegateLongPrefix.Length == 0, $"{nameof(SwitchNegateLongPrefix)} must have a length greater than 0");

        ThrowIf(GroupNames != null && GroupNames.Any(n => string.IsNullOrEmpty(n)), "Group names must have a length greater than 0");

        void ThrowIf(bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }
}
