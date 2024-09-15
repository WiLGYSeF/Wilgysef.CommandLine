namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Argument value list deserializer strategy.
/// </summary>
public abstract class ArgumentValueListDeserializerStrategy
{
    /// <summary>
    /// Deserializes <see cref="Context.Values"/> to <paramref name="result"/>.
    /// </summary>
    /// <param name="context">Deserialization context.</param>
    /// <param name="result">Deserialized value.</param>
    /// <returns><see langword="true"/> if the deserialization was successful, otherwise <see langword="false"/>.</returns>
    public abstract bool Deserialize(Context context, out object? result);

    /// <summary>
    /// Deserialization context.
    /// </summary>
    /// <param name="ArgumentToken">Argument token.</param>
    /// <param name="Type">Value type to deserialize to.</param>
    /// <param name="Values">Values to deserialize.</param>
    /// <param name="ValueName">Value name.</param>
    /// <param name="KeepFirstValue">Indicates if the first specified value should be kept for an option if it only supports one value.</param>
    /// <param name="Deserializers">Argument deserializers.</param>
    public record Context(
        ArgumentToken ArgumentToken,
        Type Type,
        IReadOnlyList<string> Values,
        string ValueName,
        bool KeepFirstValue,
        IEnumerable<IArgumentDeserializerStrategy> Deserializers);
}
