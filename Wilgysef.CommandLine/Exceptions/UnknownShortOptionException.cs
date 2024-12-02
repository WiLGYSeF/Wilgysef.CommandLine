using Wilgysef.CommandLine.Options;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown short option was encountered during parsing.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="shortOpt">Short option.</param>
/// <param name="expectedOptions">Expected options.</param>
/// <param name="innerException">Inner exception.</param>
public class UnknownShortOptionException(
    string argument,
    int argumentPosition,
    char shortOpt,
    IEnumerable<IOption> expectedOptions,
    Exception? innerException = null) : UnknownOptionException(
        argument,
        argumentPosition,
        expectedOptions,
        $"The argument \"{argument}\" at position {argumentPosition} contains an unknown option {shortOpt}",
        innerException)
{
    /// <summary>
    /// Short name.
    /// </summary>
    public char ShortName { get; } = shortOpt;
}
