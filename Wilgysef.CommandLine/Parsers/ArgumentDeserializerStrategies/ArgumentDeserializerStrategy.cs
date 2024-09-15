namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Argument deserializer strategy.
/// </summary>
/// <typeparam name="T">Deserialize target type.</typeparam>
public abstract class ArgumentDeserializerStrategy<T> : IArgumentDeserializerStrategy<T>, IArgumentDeserializerStrategy
{
    /// <inheritdoc/>
    public abstract bool MatchesType(Type type);

    /// <inheritdoc/>
    public abstract T? Deserialize(Type type, string value);

    /// <inheritdoc/>
    object? IArgumentDeserializerStrategy.Deserialize(Type type, string value)
        => Deserialize(type, value);
}
