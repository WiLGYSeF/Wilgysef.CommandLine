using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command execution context.
/// </summary>
public interface ICommandExecutionContext
{
    /// <summary>
    /// Tokenized arguments.
    /// </summary>
    TokenizedArguments TokenizedArguments { get; }

    /// <summary>
    /// Arguments.
    /// </summary>
    IEnumerable<string>? Arguments { get; }

    /// <summary>
    /// Cancellation token source.
    /// </summary>
    CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Current argument group.
    /// </summary>
    ArgumentTokenGroup ArgumentGroup { get; }

    /// <summary>
    /// Exit code to use after execution.
    /// </summary>
    int ExitCode { get; set; }

    /// <summary>
    /// Current command name.
    /// </summary>
    string? Command { get; }

    /// <summary>
    /// Command argument position.
    /// </summary>
    int ArgumentPosition { get; }
}
