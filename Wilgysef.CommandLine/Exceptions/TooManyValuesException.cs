namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if too many values are given to an argument group instance.
/// </summary>
public class TooManyValuesException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyValuesException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="maxValuesExpected">Maximum values expected.</param>
    /// <param name="unexpectedValue">Unexpected value.</param>
    public TooManyValuesException(string argument, int argumentPosition, int maxValuesExpected, string unexpectedValue)
        : base(argument, argumentPosition, $"Expected at most {maxValuesExpected} values, but found \"{unexpectedValue}\"")
    {
        MaxValuesExpected = maxValuesExpected;
        UnexpectedValue = unexpectedValue;
    }

    /// <summary>
    /// Maximum values expected.
    /// </summary>
    public int MaxValuesExpected { get; }

    /// <summary>
    /// Unexpected value.
    /// </summary>
    public string UnexpectedValue { get; }
}
