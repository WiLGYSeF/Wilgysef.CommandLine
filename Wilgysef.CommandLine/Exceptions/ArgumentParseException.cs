namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument parse exception occurs.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="message">Message.</param>
/// <param name="innerException">Inner exception.</param>
public class ArgumentParseException(
    string argument,
    int argumentPosition,
    string message,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>
    /// Argument.
    /// </summary>
    public string Argument { get; } = argument;

    /// <summary>
    /// The argument's position in the list, or <c>-1</c> if it is not in the list.
    /// </summary>
    public int ArgumentPosition { get; } = argumentPosition;
}
