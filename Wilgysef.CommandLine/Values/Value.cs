using System.Diagnostics.CodeAnalysis;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Generators;

namespace Wilgysef.CommandLine.Values;

/// <summary>
/// Used to parse argument values at positions.
/// </summary>
[GenerateFluentPattern]
public class Value : IValue
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

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public string? ValueName { get; set; }

    /// <inheritdoc/>
    public int StartIndex { get; set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Validate()
    {
        ThrowIf(EndIndex.HasValue && StartIndex > EndIndex.Value, "Start index cannot be greater than end index");

        void ThrowIf([DoesNotReturnIf(true)] bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }
}
