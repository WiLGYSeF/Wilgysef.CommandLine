namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to designate a property as a <see cref="Value"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAttribute"/> class.
    /// </summary>
    /// <param name="position">Value position.</param>
    public ValueAttribute(int position)
    {
        Min = position;
        Max = position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAttribute"/> class.
    /// </summary>
    /// <param name="min">Value position range minimum.</param>
    /// <param name="max">Value position range maximum.</param>
    public ValueAttribute(int min, int max)
    {
        Min = min;
        Max = max == int.MaxValue ? null : max;
    }

    /// <summary>
    /// Value description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Value display name.
    /// </summary>
    public string? ValueName { get; set; }

    /// <summary>
    /// Value position range minimum.
    /// </summary>
    public int Min { get; }

    /// <summary>
    /// Value position range maximum.
    /// </summary>
    public int? Max { get; }
}
