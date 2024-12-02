using System.Diagnostics.CodeAnalysis;
using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Options;

/// <summary>
/// Used to parse argument options.
/// </summary>
public class Option : IOption
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

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public bool Hidden { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? ValueNames { get; set; }

    /// <inheritdoc/>
    public ICollection<char>? ShortNames { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? LongNames { get; set; }

    /// <inheritdoc/>
    public string? ShortNamePrefix { get; set; }

    /// <inheritdoc/>
    public string? LongNamePrefix { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? KeyValueSeparators { get; set; }

    /// <inheritdoc/>
    public ValueRange? ValueCountRange { get; set; }

    /// <inheritdoc/>
    public bool Switch { get; set; }

    /// <inheritdoc/>
    public string? SwitchNegateLongPrefix { get; set; }

    /// <inheritdoc/>
    public ICollection<char>? SwitchNegateShortNames { get; set; }

    /// <inheritdoc/>
    public bool Counter { get; set; }

    /// <inheritdoc/>
    public bool Required { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? GroupNames { get; set; }

    /// <inheritdoc/>
    public bool Unique { get; set; }

    /// <inheritdoc/>
    public bool? ShortNameImmediateValue { get; set; }

    /// <inheritdoc/>
    public bool? LongNameCaseInsensitive { get; set; }

    /// <inheritdoc/>
    public bool? KeepFirstValue { get; set; }

    /// <inheritdoc/>
    public bool HasShortNames => ShortNames != null && ShortNames.Count > 0;

    /// <inheritdoc/>
    public bool HasLongNames => LongNames != null && LongNames.Count > 0;

    /// <inheritdoc/>
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
            ValueCountRange = ValueRange.AtMost(1),
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool MatchesLongName(string argName, out bool switchNegated, bool ignoreCaseDefault = false)
    {
        if (LongNames == null)
        {
            switchNegated = false;
            return false;
        }

        var comparison = LongNameCaseInsensitive ?? ignoreCaseDefault
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

    /// <inheritdoc/>
    public string GetOptionArgument(string shortNamePrefixDefault = "-", string longNamePrefixDefault = "--")
    {
        return LongNames != null
            ? $"{LongNamePrefix ?? longNamePrefixDefault}{LongNames.First()}"
            : $"{ShortNamePrefix ?? shortNamePrefixDefault}{ShortNames!.First()}";
    }

    /// <inheritdoc/>
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

        void ThrowIf([DoesNotReturnIf(true)] bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }
}
