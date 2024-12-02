using Wilgysef.CommandLine.Commands;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an unknown command was encountered during parsing.
/// </summary>
public class UnknownCommandException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="expectedCommands">Expected commands.</param>
    public UnknownCommandException(
        string argument,
        int argumentPosition,
        IEnumerable<ICommandConfiguration> expectedCommands)
        : base(argument, argumentPosition, $"The argument \"{argument}\" at position {argumentPosition} is an unknown command")
    {
        ExpectedCommands = expectedCommands;
    }

    /// <summary>
    /// Expected commands.
    /// </summary>
    public IEnumerable<ICommandConfiguration> ExpectedCommands { get; }
}
