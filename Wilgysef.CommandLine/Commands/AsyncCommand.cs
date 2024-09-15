namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command.
/// </summary>
/// <typeparam name="T">Command settings type.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
public abstract class AsyncCommand<T> : Command<T>, IAsyncCommand<T>
#pragma warning restore SA1402 // File may only contain a single type
    where T : class
{
    /// <inheritdoc/>
    public abstract Task ExecuteAsync(CommandExecutionContext context, T settings);

    /// <inheritdoc/>
    public override void Execute(CommandExecutionContext context, T settings)
    {
        ExecuteAsync(context, settings).GetAwaiter().GetResult();
    }
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
