namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Deserialization context for <see cref="IArgumentValueListDeserializerStrategy"/>.
/// </summary>
/// <param name="ArgumentToken">Argument token.</param>
/// <param name="Type">Value type to deserialize to.</param>
/// <param name="Values">Values to deserialize.</param>
/// <param name="ValueName">Value name.</param>
/// <param name="KeepFirstValue">Indicates if the first specified value should be kept for an option if it only supports one value.</param>
/// <param name="Deserializers">Argument deserializers.</param>
public record ArgumentValueListDeserializerStrategyContext(
    ArgumentToken ArgumentToken,
    Type Type,
    IReadOnlyList<string> Values,
    string ValueName,
    bool KeepFirstValue,
    IEnumerable<IArgumentDeserializerStrategy> Deserializers);
