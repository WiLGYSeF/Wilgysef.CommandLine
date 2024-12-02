namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command interface.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
public interface ICommand<T> : ICommand
    where T : class
{
    /// <summary>
    /// Factory for the command options.
    /// </summary>
    /// <returns>Options instance.</returns>
    T OptionsFactory();

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <param name="options">Command options.</param>
    void Execute(ICommandExecutionContext context, T options);
}

/// <summary>
/// Command interface.
/// </summary>
public interface ICommand : ICommandConfiguration
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    void Execute(ICommandExecutionContext context);
}
