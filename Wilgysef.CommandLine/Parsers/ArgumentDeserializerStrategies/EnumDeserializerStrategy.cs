using Wilgysef.CommandLine.Extensions;

namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// <see cref="Enum"/> deserializer strategy.
/// </summary>
public class EnumDeserializerStrategy : ArgumentDeserializerStrategy<Enum>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type.IsEnum();

    /// <inheritdoc/>
    public override Enum? Deserialize(Type type, string value)
        => (Enum?)Enum.Parse(type.UnwrapEnum(), value, true);
}
