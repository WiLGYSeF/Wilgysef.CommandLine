using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Deserialization options.
/// </summary>
public interface IDeserializationOptions
{
    /// <summary>
    /// Argument deserializers.
    /// </summary>
    ICollection<IArgumentDeserializerStrategy> Deserializers { get; set; }

    /// <summary>
    /// Argument list deserializers.
    /// </summary>
    ICollection<ArgumentValueListDeserializerStrategy> ListDeserializers { get; set; }
}
