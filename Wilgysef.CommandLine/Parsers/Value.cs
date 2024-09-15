using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Used to parse argument values at position/range.
/// </summary>
public class Value
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="positionRange">Argument position range.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
    public Value(string name, ValueRange positionRange)
    {
        if (name.Length == 0)
        {
            throw new ArgumentException("Name length must be greater than 0", nameof(name));
        }

        Name = name;
        PositionRange = positionRange;
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
    /// Specifies the argument positions used to map values from.
    /// </summary>
    public ValueRange PositionRange { get; set; }

    /// <summary>
    /// Creates a <see cref="Value"/> with one argument at <paramref name="position"/>.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <param name="position">Argument position.</param>
    /// <returns>Value.</returns>
    public static Value Single(string name, int position)
    {
        return new Value(name, new ValueRange(position, position));
    }

    /// <summary>
    /// Validates the value.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the value is invalid.</exception>
    public void Validate()
    {
        ThrowIf(PositionRange != null && PositionRange.Max.HasValue && PositionRange.Min > PositionRange.Max, "Minimum value range cannot be greater than maximum");

        void ThrowIf(bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }
}
