using Wilgysef.CommandLine.Commands;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown command was encountered during parsing.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="expectedCommands">Expected commands.</param>
public class UnknownCommandException(
    string argument,
    int argumentPosition,
    IEnumerable<ICommandConfiguration> expectedCommands)
    : ArgumentParseException(argument, argumentPosition, $"The argument \"{argument}\" at position {argumentPosition} is an unknown command")
{
    /// <summary>
    /// Expected commands.
    /// </summary>
    public IEnumerable<ICommandConfiguration> ExpectedCommands { get; } = expectedCommands;
}
