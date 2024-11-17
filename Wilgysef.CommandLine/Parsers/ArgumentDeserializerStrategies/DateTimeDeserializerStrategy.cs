namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

/// <summary>
/// Deserializer strategies for DateTime objects.
/// </summary>
public static class DateTimeDeserializerStrategies
{
    /// <summary>
    /// Gets DateTime deserializers.
    /// </summary>
    /// <returns>Deserializers.</returns>
    public static IEnumerable<IArgumentDeserializerStrategy> GetDateTimeDeserializerStrategies()
    {
        yield return new DateTimeDeserializerStrategy();
        yield return new DateTimeOffsetDeserializerStrategy();
        yield return new DateOnlyDeserializerStrategy();
        yield return new TimeOnlyDeserializerStrategy();
    }
}

/// <summary>
/// <see cref="DateTime"/> deserializer strategy.
/// </summary>
public class DateTimeDeserializerStrategy : ArgumentDeserializerStrategy<DateTime>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(DateTime) || type == typeof(DateTime?);

    /// <inheritdoc/>
    public override DateTime Deserialize(Type type, string value)
        => DateTime.Parse(value);
}

/// <summary>
/// <see cref="DateTimeOffset"/> deserializer strategy.
/// </summary>
public class DateTimeOffsetDeserializerStrategy : ArgumentDeserializerStrategy<DateTimeOffset>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);

    /// <inheritdoc/>
    public override DateTimeOffset Deserialize(Type type, string value)
        => DateTimeOffset.Parse(value);
}

/// <summary>
/// <see cref="DateOnly"/> deserializer strategy.
/// </summary>
public class DateOnlyDeserializerStrategy : ArgumentDeserializerStrategy<DateOnly>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(DateOnly) || type == typeof(DateOnly?);

    /// <inheritdoc/>
    public override DateOnly Deserialize(Type type, string value)
        => DateOnly.Parse(value);
}

/// <summary>
/// <see cref="TimeOnly"/> deserializer strategy.
/// </summary>
public class TimeOnlyDeserializerStrategy : ArgumentDeserializerStrategy<TimeOnly>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(TimeOnly) || type == typeof(TimeOnly?);

    /// <inheritdoc/>
    public override TimeOnly Deserialize(Type type, string value)
        => TimeOnly.Parse(value);
}
