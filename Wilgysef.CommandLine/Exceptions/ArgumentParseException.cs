namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument parse exception occurs.
/// </summary>
public class ArgumentParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParseException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="message">Message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ArgumentParseException(string argument, int argumentPosition, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Argument = argument;
        ArgumentPosition = argumentPosition;
    }

    /// <summary>
    /// Argument.
    /// </summary>
    public string Argument { get; }

    /// <summary>
    /// The argument's position in the list, or <c>-1</c> if it is not in the list.
    /// </summary>
    public int ArgumentPosition { get; }
}
