namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command interface.
/// </summary>
/// <typeparam name="T">Command settings type.</typeparam>
public interface IAsyncCommand<T> : ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <param name="settings">Command settings.</param>
    /// <returns>Task.</returns>
    Task ExecuteAsync(CommandExecutionContext context, T settings);
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
