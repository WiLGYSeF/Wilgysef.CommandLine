namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument value count was out of range.
/// </summary>
public class ArgumentValuesOutOfRangeException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentValuesOutOfRangeException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument postion.</param>
    /// <param name="expectedMin">Expected minimum values.</param>
    /// <param name="expectedMax">Expected maximum values.</param>
    /// <param name="actual">Actual value count.</param>
    public ArgumentValuesOutOfRangeException(
        string argument,
        int argumentPosition,
        int expectedMin,
        int? expectedMax,
        int actual)
        : base(argument, argumentPosition, GetMessage(argument, expectedMin, expectedMax, actual))
    {
        ExpectedMinValues = expectedMin;
        ExpectedMaxValues = expectedMax;
        ActualValues = actual;
    }

    /// <summary>
    /// Expected minimum values.
    /// </summary>
    public int ExpectedMinValues { get; }

    /// <summary>
    /// Expected maximum values.
    /// </summary>
    public int? ExpectedMaxValues { get; }

    /// <summary>
    /// Actual value count.
    /// </summary>
    public int ActualValues { get; }

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
