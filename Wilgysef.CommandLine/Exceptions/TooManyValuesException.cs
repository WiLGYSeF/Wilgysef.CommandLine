namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if too many values are given to an argument group instance.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="maxValuesExpected">Maximum values expected.</param>
/// <param name="unexpectedValue">Unexpected value.</param>
public class TooManyValuesException(
    string argument,
    int argumentPosition,
    int maxValuesExpected,
    string unexpectedValue)
    : ArgumentParseException(argument, argumentPosition, $"Expected at most {maxValuesExpected} values, but found \"{unexpectedValue}\"")
{
    /// <summary>
    /// Maximum values expected.
    /// </summary>
    public int MaxValuesExpected { get; } = maxValuesExpected;

    /// <summary>
    /// Unexpected value.
    /// </summary>
    public string UnexpectedValue { get; } = unexpectedValue;
}
