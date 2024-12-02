namespace Wilgysef.CommandLine.Options;

/// <summary>
/// A value range between two numbers.
/// </summary>
/// <param name="min">Minimum value.</param>
/// <param name="max">Maximum value.</param>
public class ValueRange(int min, int? max)
{
    /// <summary>
    /// Minimum value.
    /// </summary>
    public int Min { get; set; } = min;

    /// <summary>
    /// Maximum value.
    /// </summary>
    public int? Max { get; set; } = max;

    /// <summary>
    /// Creates a <see cref="ValueRange"/> with the minimum and maximum set to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Value range.</returns>
    public static ValueRange Exactly(int value)
        => new(value, value);

    /// <summary>
    /// Creates a <see cref="ValueRange"/> with range at least <paramref name="value"/>, inclusive.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Value range.</returns>
    public static ValueRange AtLeast(int value)
        => new(value, null);

    /// <summary>
    /// Creates a <see cref="ValueRange"/> with range at most <paramref name="value"/>, inclusive.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Value range.</returns>
    public static ValueRange AtMost(int value)
        => new(0, value);

    /// <summary>
    /// Checks if <paramref name="value"/> is in between <see cref="Min"/> and <see cref="Max"/>, inclusive.
    /// If <see cref="Max"/> is <see langword="null"/>, treat it as infinity.
    /// </summary>
    /// <param name="value">Number.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is within the range, otherwise <see langword="false"/>.</returns>
    public bool InRange(int value)
    {
        if (value < Min)
        {
            return false;
        }

        return !Max.HasValue || value <= Max;
    }

    /// <summary>
    /// Checks if <paramref name="value"/> is less than <see cref="Max"/>.
    /// If <see cref="Max"/> is <see langword="null"/>, treat it as infinity.
    /// </summary>
    /// <param name="value">Number.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than the maximum, otherwise <see langword="false"/>.</returns>
    public bool UnderMax(int value)
    {
        return !Max.HasValue || value < Max;
    }
}
