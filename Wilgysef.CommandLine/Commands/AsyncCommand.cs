namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
public abstract class AsyncCommand<T> : AsyncCommand, IAsyncCommand<T>, ICommand<T>
    where T : class
{
    /// <inheritdoc/>
    public virtual T OptionsFactory()
    {
        return Activator.CreateInstance<T>();
    }

    /// <inheritdoc/>
    public abstract Task ExecuteAsync(CommandExecutionContext context, T options);

    /// <inheritdoc/>
    public override Task ExecuteAsync(CommandExecutionContext context)
        => ExecuteAsync(context, null!);

    /// <inheritdoc/>
    public virtual void Execute(CommandExecutionContext context, T options)
        => ExecuteAsync(context, options).GetAwaiter().GetResult();
}

/// <summary>
/// Asynchronous command.
/// </summary>
public abstract class AsyncCommand : Command, IAsyncCommand
{
    /// <inheritdoc/>
    public abstract Task ExecuteAsync(CommandExecutionContext context);

    /// <inheritdoc/>
    public override void Execute(CommandExecutionContext context)
        => ExecuteAsync(context).GetAwaiter().GetResult();
}
