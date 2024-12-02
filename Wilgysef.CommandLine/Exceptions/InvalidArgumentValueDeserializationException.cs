using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if a argument value deserialization exception occurs.
/// </summary>
/// <param name="argToken">Argument token.</param>
/// <param name="deserializationMessage">Deserialization message.</param>
/// <param name="innerException">Inner exception.</param>
public class InvalidArgumentValueDeserializationException(
    ArgumentToken argToken,
    string deserializationMessage,
    Exception? innerException = null)
    : ArgumentParseException(
        argToken.Argument,
        argToken.ArgumentPosition,
        $"Error with argument \"{argToken.Argument}\" at position {argToken.ArgumentPosition}: {deserializationMessage}",
        innerException)
{
    /// <summary>
    /// Deserialization message.
    /// </summary>
    public string DeserializationMessage { get; } = deserializationMessage;
}
