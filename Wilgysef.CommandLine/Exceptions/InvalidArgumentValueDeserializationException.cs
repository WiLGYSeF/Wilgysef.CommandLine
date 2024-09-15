using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if a argument value deserialization exception occurs.
/// </summary>
public class InvalidArgumentValueDeserializationException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueDeserializationException"/> class.
    /// </summary>
    /// <param name="argToken">Argument token.</param>
    /// <param name="deserializationMessage">Deserialization message.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidArgumentValueDeserializationException(
        ArgumentToken argToken,
        string deserializationMessage,
        Exception? innerException = null)
        : base(
            argToken.Argument,
            argToken.ArgumentPosition,
            $"Error with argument \"{argToken.Argument}\" at position {argToken.ArgumentPosition}: {deserializationMessage}",
            innerException)
    {
        DeserializationMessage = deserializationMessage;
    }

    /// <summary>
    /// Deserialization message.
    /// </summary>
    public string DeserializationMessage { get; }
}
