namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
/// <typeparam name="T">Command options type.</typeparam>
internal class AsyncDelegateCommand<T>(string name, Func<ICommandExecutionContext, T, Task> action) : AsyncCommand<T>
    where T : class
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override Task ExecuteAsync(ICommandExecutionContext context, T options)
        => action(context, options);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
internal class AsyncDelegateCommand(string name, Func<ICommandExecutionContext, Task> action) : AsyncCommand
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override Task ExecuteAsync(ICommandExecutionContext context)
        => action(context);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
/// <typeparam name="T">Command options type.</typeparam>
internal class DelegateCommand<T>(string name, Action<ICommandExecutionContext, T> action) : Command<T>
    where T : class
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override void Execute(ICommandExecutionContext context, T options)
        => action(context, options);
}

/// <summary>
/// Command from delegate.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="action">Action invoked on command execution.</param>
internal class DelegateCommand(string name, Action<ICommandExecutionContext> action) : Command
{
    /// <inheritdoc/>
    public override string Name => name;

    /// <inheritdoc/>
    public override void Execute(ICommandExecutionContext context)
        => action(context);
}
