using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Values;

/// <summary>
/// Used to parse argument values at positions.
/// </summary>
public interface IValue
{
    /// <summary>
    /// Value name.
    /// </summary>
    /// <remarks>
    /// Used to determine the instance property to set when parsing.
    /// </remarks>
    string Name { get; set; }

    /// <summary>
    /// Value description.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Value display name.
    /// </summary>
    string? ValueName { get; set; }

    /// <summary>
    /// Argument position start index.
    /// </summary>
    int StartIndex { get; set; }

    /// <summary>
    /// Argument position end index.
    /// </summary>
    int? EndIndex { get; set; }

    /// <summary>
    /// Validates the value.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the value is invalid.</exception>
    void Validate();
}
