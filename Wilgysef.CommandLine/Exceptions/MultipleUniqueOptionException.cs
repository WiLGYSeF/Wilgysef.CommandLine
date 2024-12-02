using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if multiple unique options are encountered during parsing.
/// </summary>
/// <param name="argToken">Argument token.</param>
/// <param name="otherArgumentPosition">Other argument position.</param>
public class MultipleUniqueOptionException(ArgumentToken argToken, int otherArgumentPosition)
    : ArgumentParseException(
        argToken.Argument,
        argToken.ArgumentPosition,
        $"The argument \"{argToken.Argument}\" at position {otherArgumentPosition} was already specified at position {otherArgumentPosition}")
{
    /// <summary>
    /// Other argument position.
    /// </summary>
    public int OtherArgumentPosition { get; } = otherArgumentPosition;
}
