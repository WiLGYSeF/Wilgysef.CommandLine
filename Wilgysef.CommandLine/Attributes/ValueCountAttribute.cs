namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to set option value counts.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValueCountAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCountAttribute"/> class.
    /// </summary>
    /// <param name="valueNames">Value names.</param>
    public ValueCountAttribute(params string[] valueNames)
        : this(valueNames.Length, valueNames.Length, null, valueNames)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCountAttribute"/> class.
    /// </summary>
    /// <param name="count">Value count.</param>
    /// <param name="valueNames">Value names.</param>
    public ValueCountAttribute(int count, params string[] valueNames)
        : this(count, count, null, valueNames)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCountAttribute"/> class.
    /// </summary>
    /// <param name="min">Value count range minimum.</param>
    /// <param name="max">Value count range maximum.</param>
    /// <param name="valueNames">Value names.</param>
    public ValueCountAttribute(int min, int max, params string[] valueNames)
        : this(min, max, null, valueNames)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCountAttribute"/> class.
    /// </summary>
    /// <param name="count">Value count.</param>
    /// <param name="keepFirstValue">Indicates if the first specified value should be kept for an option if it only supports one value.</param>
    /// <param name="valueNames">Value names.</param>
    public ValueCountAttribute(int count, bool keepFirstValue, params string[] valueNames)
        : this(count, count, keepFirstValue, valueNames)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCountAttribute"/> class.
    /// </summary>
    /// <param name="min">Value count range minimum.</param>
    /// <param name="max">Value count range maximum.</param>
    /// <param name="keepFirstValue">Indicates if the first specified value should be kept for an option if it only supports one value.</param>
    /// <param name="valueNames">Value names.</param>
    // do not remove casts in this() constructor
    public ValueCountAttribute(int min, int max, bool keepFirstValue, params string[] valueNames)
        : this(min, (int?)max, (bool?)keepFirstValue, valueNames)
    {
    }

    private ValueCountAttribute(int min, int? max, bool? keepFirstValue, params string[] valueNames)
    {
        Min = min;
        Max = max;
        KeepFirstValue = keepFirstValue;
        ValueNames = valueNames;
    }

    /// <summary>
    /// Value count range minimum.
    /// </summary>
    public int Min { get; }

    /// <summary>
    /// Value count range maximum.
    /// </summary>
    public int? Max { get; }

    /// <summary>
    /// Value names to display in help providers.
    /// </summary>
    public IReadOnlyList<string> ValueNames { get; }

    /// <summary>
    /// Key-value separator.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--key=value</c>, <c>"="</c> is the key-value separator.
    /// </remarks>
    public string? KeyValueSeparator { get; set; }

    /// <summary>
    /// Indicates if the first specified value should be kept for an option if it only supports one value.
    /// </summary>
    public bool? KeepFirstValue { get; }
}
