namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// A value range between two numbers.
/// </summary>
public class ValueRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueRange"/> class.
    /// </summary>
    /// <param name="value">Value.</param>
    public ValueRange(int value)
        : this(value, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueRange"/> class.
    /// </summary>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    public ValueRange(int min, int? max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Minimum value.
    /// </summary>
    public int Min { get; set; }

    /// <summary>
    /// Maximum value.
    /// </summary>
    public int? Max { get; set; }

    /// <summary>
    /// Checks if <paramref name="num"/> is in between <see cref="Min"/> and <see cref="Max"/>, inclusive.
    /// If <see cref="Max"/> is <see langword="null"/>, treat it as infinity.
    /// </summary>
    /// <param name="num">Number.</param>
    /// <returns><see langword="true"/> if <paramref name="num"/> is within the range, otherwise <see langword="false"/>.</returns>
    public bool InRange(int num)
    {
        if (num < Min)
        {
            return false;
        }

        return !Max.HasValue || num <= Max;
    }

    /// <summary>
    /// Checks if <paramref name="num"/> is less than <see cref="Max"/>.
    /// If <see cref="Max"/> is <see langword="null"/>, treat it as infinity.
    /// </summary>
    /// <param name="num">Number.</param>
    /// <returns><see langword="true"/> if <paramref name="num"/> is less than the maximum, otherwise <see langword="false"/>.</returns>
    public bool UnderMax(int num)
    {
        return !Max.HasValue || num < Max;
    }
}
