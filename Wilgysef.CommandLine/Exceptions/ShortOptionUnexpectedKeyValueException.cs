namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if a short option unexpectedly has a key-value pair.
/// </summary>
public class ShortOptionUnexpectedKeyValueException : OptionUnexpectedKeyValueException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortOptionUnexpectedKeyValueException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="shortOpt">Short option.</param>
    public ShortOptionUnexpectedKeyValueException(string argument, int argumentPosition, char shortOpt)
        : base(argument, argumentPosition, $"The argument {shortOpt} in \"{argument}\" at position {argumentPosition} is not expected to have a value")
    {
        ShortName = shortOpt;
    }

    /// <summary>
    /// Short name.
    /// </summary>
    public char ShortName { get; }
}
