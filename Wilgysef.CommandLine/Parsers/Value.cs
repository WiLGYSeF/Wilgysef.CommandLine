using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Used to parse argument values at positions.
/// </summary>
public class Value
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="startIndex">Argument position start index.</param>
    /// <param name="endIndex">Argument position end index.</param>
    public Value(string name, int startIndex, int? endIndex)
    {
        if (name.Length == 0)
        {
            throw new ArgumentException("Name length must be greater than 0", nameof(name));
        }

        Name = name;
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>
    /// Value name.
    /// </summary>
    /// <remarks>
    /// Used to determine the instance property to set when parsing.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Value description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Value display name.
    /// </summary>
    public string? ValueName { get; set; }

    /// <summary>
    /// Argument position start index.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// Argument position end index.
    /// </summary>
    public int? EndIndex { get; set; }

    /// <summary>
    /// Creates a <see cref="Value"/> with one argument at <paramref name="index"/>.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="index">Argument position index.</param>
    /// <returns>Value.</returns>
    public static Value Single(string name, int index)
        => new(name, index, index);

    /// <summary>
    /// Creates a <see cref="Value"/> with argument position indexes at <paramref name="index"/> and beyond.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="index">Argument position start index.</param>
    /// <returns>Value.</returns>
    public static Value StartingAt(string name, int index)
        => new(name, index, null);

    /// <summary>
    /// Creates a <see cref="Value"/> with argument position indexes at <paramref name="index"/> for <paramref name="count"/> arguments.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="index">Argument position start index.</param>
    /// <param name="count">Number of arguments to include.</param>
    /// <returns>Value.</returns>
    public static Value AtForNext(string name, int index, int count)
        => new(name, index, index + count);

    /// <summary>
    /// Creates a <see cref="Value"/> with all argument position indexes.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <returns>Value.</returns>
    public static Value All(string name)
        => new(name, 0, null);

    /// <summary>
    /// Validates the value.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the value is invalid.</exception>
    public void Validate()
    {
        ThrowIf(EndIndex.HasValue && StartIndex > EndIndex.Value, "Start index cannot be greater than end index");

        void ThrowIf(bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }
}
