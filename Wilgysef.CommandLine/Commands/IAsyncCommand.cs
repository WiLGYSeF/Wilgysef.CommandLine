namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command interface.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
public interface IAsyncCommand<T> : ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <param name="options">Command options.</param>
    /// <returns>Task.</returns>
    Task ExecuteAsync(CommandExecutionContext context, T options);
}

/// <summary>
/// Asynchronous command interface.
/// </summary>
public interface IAsyncCommand : ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <returns>Task.</returns>
    Task ExecuteAsync(CommandExecutionContext context);
}
