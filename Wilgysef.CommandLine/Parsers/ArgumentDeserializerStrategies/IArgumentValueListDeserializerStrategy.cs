namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Argument value list deserializer strategy.
/// </summary>
public interface IArgumentValueListDeserializerStrategy
{
    /// <summary>
    /// Deserializes <see cref="ArgumentValueListDeserializerStrategyContext.Values"/> to <paramref name="result"/>.
    /// </summary>
    /// <param name="context">Deserialization context.</param>
    /// <param name="result">Deserialized value.</param>
    /// <returns><see langword="true"/> if the deserialization was successful, otherwise <see langword="false"/>.</returns>
    bool Deserialize(ArgumentValueListDeserializerStrategyContext context, out object? result);
}
