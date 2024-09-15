namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
public abstract class AsyncCommand<T> : Command<T>, IAsyncCommand<T>
#pragma warning restore SA1402 // File may only contain a single type
    where T : class
{
    /// <inheritdoc/>
    public abstract Task ExecuteAsync(CommandExecutionContext context, T options);

    /// <inheritdoc/>
    public override void Execute(CommandExecutionContext context, T options)
    {
        ExecuteAsync(context, options).GetAwaiter().GetResult();
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
