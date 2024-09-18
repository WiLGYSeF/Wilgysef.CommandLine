using Wilgysef.CommandLine.Parsers;

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
    /// <param name="index">Value position index.</param>
    public ValueAttribute(int index)
    {
        StartIndex = index;
        EndIndex = index;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAttribute"/> class.
    /// </summary>
    /// <param name="startIndex">Value position start index.</param>
    /// <param name="endIndex">Value position end index.</param>
    public ValueAttribute(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex == int.MaxValue ? null : endIndex;
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
    /// Value position start index.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Value position end index.
    /// </summary>
    public int? EndIndex { get; }
}
