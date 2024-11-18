using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command execution context.
/// </summary>
/// <param name="tokenizedArguments">Tokenized arguments.</param>
/// <param name="args">Arguments.</param>
/// <param name="argGroup">Current argument group.</param>
/// <param name="cancellationTokenSource">Cancellation token source.</param>
public class CommandExecutionContext(
    TokenizedArguments tokenizedArguments,
    IEnumerable<string>? args,
    ArgumentTokenGroup argGroup,
    CancellationTokenSource cancellationTokenSource)
    : ICommandExecutionContext
{
    /// <inheritdoc/>
    public TokenizedArguments TokenizedArguments { get; } = tokenizedArguments;

    /// <inheritdoc/>
    public IEnumerable<string>? Arguments { get; } = args;

    /// <inheritdoc/>
    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource;

    /// <inheritdoc/>
    public ArgumentTokenGroup ArgumentGroup { get; internal set; } = argGroup;

    /// <inheritdoc/>
    public int ExitCode { get; set; }

    /// <inheritdoc/>
    public string? Command => ArgumentGroup.Command?.Name;

    /// <inheritdoc/>
    public int ArgumentPosition => ArgumentGroup.CommandMatch?.ArgumentPosition ?? 0;
}
