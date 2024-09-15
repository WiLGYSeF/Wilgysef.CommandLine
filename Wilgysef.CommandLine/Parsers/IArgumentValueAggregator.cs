using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Parsers;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Deserialized argument instance value aggregator.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
public interface IArgumentValueAggregator<T> : IArgumentValueAggregator
    where T : class
{
    /// <summary>
    /// Sets the instance value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns><see langword="true"/> if the value was set, otherwise <see langword="false"/>.</returns>
    bool SetValue(ArgumentValueAggregatorContext<T> context);
}

/// <summary>
/// Deserialized argument instance value aggregator.
/// </summary>
public interface IArgumentValueAggregator
{
    /// <summary>
    /// Checks if the instance type matches the value aggregator.
    /// </summary>
    /// <param name="type">Instance type.</param>
    /// <returns><see langword="true"/> if the value type matches, otherwise <see langword="false"/>.</returns>
    bool MatchesInstanceType(Type type);

    /// <summary>
    /// Sets the instance value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns><see langword="true"/> if the value was set, otherwise <see langword="false"/>.</returns>
    bool SetValue(ArgumentValueAggregatorContext context);
}

/// <summary>
/// Argument value aggregator context.
/// </summary>
/// <param name="Instance">Instance.</param>
/// <param name="ArgumentToken">Argument token.</param>
/// <param name="InstanceValueHandler">Instance value handler.</param>
/// <param name="ValueName">Value name.</param>
/// <param name="Value">Value to set.</param>
/// <param name="SetValue">The base method for setting <paramref name="Value"/> to <paramref name="Instance"/>.</param>
/// <typeparam name="T">Instance type.</typeparam>
public record ArgumentValueAggregatorContext<T>(
    T Instance,
    ArgumentToken ArgumentToken,
    IInstanceValueHandler<T> InstanceValueHandler,
    string ValueName,
    object? Value,
    Action<string, object?> SetValue)
    where T : class;

/// <summary>
/// Argument value aggregator context.
/// </summary>
/// <param name="Instance">Instance.</param>
/// <param name="ArgumentToken">Argument token.</param>
/// <param name="InstanceValueHandler">Instance value handler.</param>
/// <param name="ValueName">Value name.</param>
/// <param name="Value">Value to set.</param>
/// <param name="SetValue">The base method for setting <paramref name="Value"/> to <paramref name="Instance"/>.</param>
public record ArgumentValueAggregatorContext(
    object Instance,
    ArgumentToken ArgumentToken,
    IInstanceValueHandler InstanceValueHandler,
    string ValueName,
    object? Value,
    Action<string, object?> SetValue);

#pragma warning restore SA1402 // File may only contain a single type
