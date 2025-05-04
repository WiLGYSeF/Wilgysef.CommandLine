using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Generators;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Asynchronous command.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
[GenerateFluentPattern]
public abstract class AsyncCommand<T> : AsyncCommand, IAsyncCommand<T>, ICommand<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
    /// </summary>
    protected AsyncCommand()
    {
        if (AddOptionsValues)
        {
            this.WithOptionsValuesFrom<T>();
        }
    }

    /// <summary>
    /// Indicates if the options and values attributes should be added from <typeparamref name="T"/>.
    /// </summary>
    protected virtual bool AddOptionsValues => true;

    /// <inheritdoc/>
    public virtual T OptionsFactory()
    {
        return Activator.CreateInstance<T>();
    }

    /// <inheritdoc/>
    public abstract Task ExecuteAsync(ICommandExecutionContext context, T options);

    /// <inheritdoc/>
    public override Task ExecuteAsync(ICommandExecutionContext context)
        => ExecuteAsync(context, null!);

    /// <inheritdoc/>
    public virtual void Execute(ICommandExecutionContext context, T options)
        => ExecuteAsync(context, options).GetAwaiter().GetResult();
}

/// <summary>
/// Asynchronous command.
/// </summary>
[GenerateFluentPattern]
public abstract class AsyncCommand : Command, IAsyncCommand
{
    /// <inheritdoc/>
    public abstract Task ExecuteAsync(ICommandExecutionContext context);

    /// <inheritdoc/>
    public override void Execute(ICommandExecutionContext context)
        => ExecuteAsync(context).GetAwaiter().GetResult();
}
