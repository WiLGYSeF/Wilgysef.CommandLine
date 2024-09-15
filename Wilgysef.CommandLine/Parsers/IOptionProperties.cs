namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Option properties.
/// </summary>
public interface IOptionProperties
{
    /// <summary>
    /// Short name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-p</c>, <c>"-"</c> is the short name prefix.
    /// </remarks>
    string? ShortNamePrefix { get; set; }

    /// <summary>
    /// Long name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--option</c>, <c>"--"</c> is the long name prefix.
    /// </remarks>
    string? LongNamePrefix { get; set; }

    /// <summary>
    /// Key-value separators.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--key=value</c>, <c>"="</c> is the key-value separator.
    /// </remarks>
    ICollection<string>? KeyValueSeparators { get; set; }

    /// <summary>
    /// Indicates if short name options should use immediate characters as a value if applicable.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P3</c>, <c>3</c> is considered the immediate value.
    /// </remarks>
    bool? ShortNameImmediateValue { get; set; }

    /// <summary>
    /// Indicates if the option long names are case insensitive.
    /// </summary>
    bool? LongNameCaseInsensitive { get; set; }

    /// <summary>
    /// Indicates if the first specified value should be kept for an option if it only supports one value.
    /// </summary>
    bool? KeepFirstValue { get; set; }
}
