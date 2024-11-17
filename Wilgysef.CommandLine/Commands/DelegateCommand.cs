namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
/// <typeparam name="T">Command options type.</typeparam>
public class AsyncDelegateCommand<T>(string name, Func<CommandExecutionContext, T, Task> action) : AsyncCommand<T>
    where T : class
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override Task ExecuteAsync(CommandExecutionContext context, T options)
        => action(context, options);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
public class AsyncDelegateCommand(string name, Func<CommandExecutionContext, Task> action) : AsyncCommand
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override Task ExecuteAsync(CommandExecutionContext context)
        => action(context);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
/// <typeparam name="T">Command options type.</typeparam>
public class DelegateCommand<T>(string name, Action<CommandExecutionContext, T> action) : Command<T>
    where T : class
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override void Execute(CommandExecutionContext context, T options)
        => action(context, options);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
public class DelegateCommand(string name, Action<CommandExecutionContext> action) : Command
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override void Execute(CommandExecutionContext context)
        => action(context);
}
