using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if multiple unique options are encountered during parsing.
/// </summary>
public class MultipleUniqueOptionException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleUniqueOptionException"/> class.
    /// </summary>
    /// <param name="argToken">Argument token.</param>
    /// <param name="otherArgumentPosition">Other argument position.</param>
    public MultipleUniqueOptionException(ArgumentToken argToken, int otherArgumentPosition)
        : base(
            argToken.Argument,
            argToken.ArgumentPosition,
            $"The argument \"{argToken.Argument}\" at position {otherArgumentPosition} was already specified at position {otherArgumentPosition}")
    {
        OtherArgumentPosition = otherArgumentPosition;
    }

    /// <summary>
    /// Other argument position.
    /// </summary>
    public int OtherArgumentPosition { get; }
}
