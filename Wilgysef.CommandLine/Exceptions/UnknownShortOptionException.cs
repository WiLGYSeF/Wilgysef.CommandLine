using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown short option was encountered during parsing.
/// </summary>
public class UnknownShortOptionException : UnknownOptionException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownShortOptionException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="shortOpt">Short option.</param>
    /// <param name="expectedOptions">Expected options.</param>
    /// <param name="innerException">Inner exception.</param>
    public UnknownShortOptionException(
        string argument,
        int argumentPosition,
        char shortOpt,
        IEnumerable<Option> expectedOptions,
        Exception? innerException = null)
        : base(
            argument,
            argumentPosition,
            expectedOptions,
            $"The argument \"{argument}\" at position {argumentPosition} contains an unknown option {shortOpt}",
            innerException)
    {
        ShortName = shortOpt;
    }

    /// <summary>
    /// Short name.
    /// </summary>
    public char ShortName { get; }
}
