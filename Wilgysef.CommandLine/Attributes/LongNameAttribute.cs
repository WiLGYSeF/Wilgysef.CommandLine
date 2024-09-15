namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to set option long names.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LongNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LongNameAttribute"/> class.
    /// </summary>
    /// <param name="longNames">Long names.</param>
    public LongNameAttribute(params string[] longNames)
    {
        LongNameCaseInsensitive = null;
        LongNames = longNames;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LongNameAttribute"/> class.
    /// </summary>
    /// <param name="longNameCaseInsensitive">Indicates if the option long names are case insensitive.</param>
    /// <param name="longNames">Long names.</param>
    public LongNameAttribute(bool longNameCaseInsensitive, params string[] longNames)
    {
        LongNameCaseInsensitive = longNameCaseInsensitive;
        LongNames = longNames;
    }

    /// <summary>
    /// Option long names.
    /// </summary>
    public IReadOnlyList<string> LongNames { get; }

    /// <summary>
    /// Indicates if the option long names are case insensitive.
    /// </summary>
    public bool? LongNameCaseInsensitive { get; }

    /// <summary>
    /// Long name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--option</c>, <c>"--"</c> is the long name prefix.
    /// </remarks>
    public string? LongNamePrefix { get; set; }
}
