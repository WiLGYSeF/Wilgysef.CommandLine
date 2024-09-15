namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to set short names for an <see cref="OptionAttribute"/> property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ShortNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortNameAttribute"/> class.
    /// </summary>
    /// <param name="shortNames">Option short names.</param>
    public ShortNameAttribute(params char[] shortNames)
    {
        ShortNameImmediateValue = null;
        ShortNames = shortNames;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortNameAttribute"/> class.
    /// </summary>
    /// <param name="shortNameImmediateValue">Indicates if short name options should use immediate characters as a value if applicable.</param>
    /// <param name="shortNames">Option short names.</param>
    public ShortNameAttribute(bool shortNameImmediateValue, params char[] shortNames)
    {
        ShortNameImmediateValue = shortNameImmediateValue;
        ShortNames = shortNames;
    }

    /// <summary>
    /// Option short names.
    /// </summary>
    public IReadOnlyList<char> ShortNames { get; }

    /// <summary>
    /// Indicates if short name options should use immediate characters as a value if applicable.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P3</c>, <c>3</c> is considered the immediate value.
    /// </remarks>
    public bool? ShortNameImmediateValue { get; }

    /// <summary>
    /// Short name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-p</c>, <c>"-"</c> is the short name prefix.
    /// </remarks>
    public string? ShortNamePrefix { get; set; }
}
