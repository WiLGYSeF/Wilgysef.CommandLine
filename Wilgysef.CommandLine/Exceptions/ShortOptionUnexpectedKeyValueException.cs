namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if a short option unexpectedly has a key-value pair.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="shortOpt">Short option.</param>
public class ShortOptionUnexpectedKeyValueException(string argument, int argumentPosition, char shortOpt)
    : OptionUnexpectedKeyValueException(argument, argumentPosition, $"The argument {shortOpt} in \"{argument}\" at position {argumentPosition} is not expected to have a value")
{
    /// <summary>
    /// Short name.
    /// </summary>
    public char ShortName { get; } = shortOpt;
}
