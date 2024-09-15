using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command execution context.
/// </summary>
public class CommandExecutionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandExecutionContext"/> class.
    /// </summary>
    /// <param name="tokenizedArguments">Tokenized arguments.</param>
    /// <param name="args">Arguments.</param>
    /// <param name="argGroup">Current argument group.</param>
    /// <param name="cancellationTokenSource">Cancellation token source.</param>
    public CommandExecutionContext(
        TokenizedArguments tokenizedArguments,
        IEnumerable<string>? args,
        ArgumentTokenGroup argGroup,
        CancellationTokenSource cancellationTokenSource)
    {
        TokenizedArguments = tokenizedArguments;
        Arguments = args;
        ArgumentGroup = argGroup;
        CancellationTokenSource = cancellationTokenSource;
    }

    /// <summary>
    /// Tokenized arguments.
    /// </summary>
    public TokenizedArguments TokenizedArguments { get; }

    /// <summary>
    /// Arguments.
    /// </summary>
    public IEnumerable<string>? Arguments { get; }

    /// <summary>
    /// Cancellation token source.
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Current argument group.
    /// </summary>
    public ArgumentTokenGroup ArgumentGroup { get; set; }

    /// <summary>
    /// Exit code to use after execution.
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// Current command name.
    /// </summary>
    public string? Command => ArgumentGroup.Command?.Name;

    /// <summary>
    /// Command argument position.
    /// </summary>
    public int ArgumentPosition => ArgumentGroup.CommandMatch?.ArgumentPosition ?? 0;
}
