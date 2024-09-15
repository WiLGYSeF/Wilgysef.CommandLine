namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to designate an option as a switch.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwitchAttribute : Attribute
{
    /// <summary>
    /// Switch negation long prefix used to configure a negatable option.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--no-cache</c>, <c>"no-"</c> is the negated switch long prefix.
    /// </remarks>
    public string? SwitchNegateLongPrefix { get; set; }

    /// <summary>
    /// Switch negation short name.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P</c>, <c>P</c> is the negated short name.
    /// </remarks>
    public char SwitchNegateShortName { get; set; }
}
