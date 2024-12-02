namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an option value count was out of range.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument postion.</param>
/// <param name="expectedMin">Expected minimum values.</param>
/// <param name="expectedMax">Expected maximum values.</param>
/// <param name="actual">Actual value count.</param>
public class OptionValuesOutOfRangeException(
    string argument,
    int argumentPosition,
    int expectedMin,
    int? expectedMax,
    int actual)
    : ArgumentParseException(
        argument,
        argumentPosition,
        GetMessage(argument, expectedMin, expectedMax, actual))
{
    /// <summary>
    /// Expected minimum values.
    /// </summary>
    public int ExpectedMinValues { get; } = expectedMin;

    /// <summary>
    /// Expected maximum values.
    /// </summary>
    public int? ExpectedMaxValues { get; } = expectedMax;

    /// <summary>
    /// Actual value count.
    /// </summary>
    public int ActualValues { get; } = actual;

    private static string GetMessage(
        string argument,
        int expectedMin,
        int? expectedMax,
        int actual)
    {
        if (expectedMin == expectedMax)
        {
            return $"\"{argument}\" expected {expectedMin} values, but got {actual}";
        }

        if (expectedMax.HasValue)
        {
            return $"\"{argument}\" expected between {expectedMin} and {expectedMax} values, but got {actual}";
        }

        return $"\"{argument}\" expected at least {expectedMin} values, but got {actual}";
    }
}
