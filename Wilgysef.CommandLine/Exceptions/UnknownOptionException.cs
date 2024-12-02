using Wilgysef.CommandLine.Options;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown option was encountered during parsing.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="expectedOptions">Expected options.</param>
/// <param name="message">Message.</param>
/// <param name="innerException">Inner exception.</param>
public class UnknownOptionException(
    string argument,
    int argumentPosition,
    IEnumerable<IOption> expectedOptions,
    string? message = null,
    Exception? innerException = null)
    : ArgumentParseException(
        argument,
        argumentPosition,
        message ?? $"The argument \"{argument}\" at position {argumentPosition} is an unknown option",
        innerException)
{
    /// <summary>
    /// Expected options.
    /// </summary>
    public IEnumerable<IOption> ExpectedOptions { get; } = expectedOptions;
}
