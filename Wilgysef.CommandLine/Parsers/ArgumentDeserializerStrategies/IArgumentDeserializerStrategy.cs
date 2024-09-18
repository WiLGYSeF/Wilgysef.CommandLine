namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Interface for generic argument deserializer strategies.
/// </summary>
/// <typeparam name="T">Deserialization type.</typeparam>
public interface IArgumentDeserializerStrategy<T> : IArgumentDeserializerStrategy
{
    /// <summary>
    /// Deserializes <paramref name="value"/> to an object of type <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type to deserialize to.</param>
    /// <param name="value">Value to deserialize.</param>
    /// <returns>Deserialized value.</returns>
    new T? Deserialize(Type type, string value);
}

/// <summary>
/// Interface for argument deserializer strategies.
/// </summary>
public interface IArgumentDeserializerStrategy
{
    /// <summary>
    /// Checks whether the type <paramref name="type"/> matches the deserializer strategy.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns><see langword="true"/> if the type matches the deserializer strategy, otherwise <see langword="false"/>.</returns>
    bool MatchesType(Type type);

    /// <summary>
    /// Deserializes <paramref name="value"/> to an object of type <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type to deserialize to.</param>
    /// <param name="value">Value to deserialize.</param>
    /// <returns>Deserialized value.</returns>
    object? Deserialize(Type type, string value);
}
