using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Parsers;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Deserialized argument instance value aggregator.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
public abstract class ArgumentValueAggregator<T> : ArgumentValueAggregator, IArgumentValueAggregator<T>
    where T : class
{
    /// <summary>
    /// Creates a <see cref="ArgumentValueAggregator{T}"/> from a delegate function.
    /// </summary>
    /// <param name="setValue">Set value func.</param>
    /// <returns>Argument value aggregator.</returns>
    public static ArgumentValueAggregator<T> Create(Func<ArgumentValueAggregatorContext<T>, bool> setValue)
        => new DelegateArgumentValueAggregator<T>(setValue);

    /// <inheritdoc/>
    public override bool MatchesInstanceType(Type type) => type == typeof(T);

    /// <summary>
    /// Sets the instance value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns><see langword="true"/> if the value was set, otherwise <see langword="false"/>.</returns>
    public abstract bool SetValue(ArgumentValueAggregatorContext<T> context);

    /// <inheritdoc/>
    public override bool SetValue(ArgumentValueAggregatorContext context)
        => SetValue(new ArgumentValueAggregatorContext<T>(
            (T)context.Instance,
            context.ArgumentToken,
            (IInstanceValueHandler<T>)context.InstanceValueHandler,
            context.ValueName,
            context.Value,
            context.SetValue));
}

/// <summary>
/// Deserialized argument instance value aggregator.
/// </summary>
public abstract class ArgumentValueAggregator : IArgumentValueAggregator
{
    /// <inheritdoc/>
    public abstract bool MatchesInstanceType(Type type);

    /// <inheritdoc/>
    public abstract bool SetValue(ArgumentValueAggregatorContext context);
}

/// <summary>
/// DelegateArgumentValueAggregator.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
/// <param name="setValue">Set value function.</param>
internal class DelegateArgumentValueAggregator<T>(Func<ArgumentValueAggregatorContext<T>, bool> setValue) : ArgumentValueAggregator<T>
    where T : class
{
    /// <inheritdoc/>
    public override bool SetValue(ArgumentValueAggregatorContext<T> context)
        => setValue(context);
}

#pragma warning restore SA1402 // File may only contain a single type
