namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument unexpectedly has a key-value pair.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="message">Message.</param>
public class OptionUnexpectedKeyValueException(string argument, int argumentPosition, string? message = null)
    : ArgumentParseException(argument, argumentPosition, message ?? $"The argument \"{argument}\" at position {argumentPosition} is not expected to have a value")
{
}
