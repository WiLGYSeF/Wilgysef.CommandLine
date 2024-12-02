using Wilgysef.CommandLine.Options;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown option was encountered during parsing.
/// </summary>
public class UnknownOptionException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownOptionException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="expectedOptions">Expected options.</param>
    /// <param name="message">Message.</param>
    /// <param name="innerException">Inner exception.</param>
    public UnknownOptionException(
        string argument,
        int argumentPosition,
        IEnumerable<Option> expectedOptions,
        string? message = null,
        Exception? innerException = null)
        : base(argument, argumentPosition, message ?? $"The argument \"{argument}\" at position {argumentPosition} is an unknown option", innerException)
    {
        ExpectedOptions = expectedOptions;
    }

    /// <summary>
    /// Expected options.
    /// </summary>
    public IEnumerable<Option> ExpectedOptions { get; }
}
