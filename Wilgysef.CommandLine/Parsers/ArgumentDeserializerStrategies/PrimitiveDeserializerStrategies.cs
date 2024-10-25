namespace Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Deserializer strategies for primitives.
/// </summary>
public static class PrimitiveDeserializerStrategies
{
    /// <summary>
    /// Gets primitive deserializers.
    /// </summary>
    /// <returns>Primitive deserializers.</returns>
    public static IEnumerable<IArgumentDeserializerStrategy> GetPrimitiveDeserializerStrategies()
    {
        yield return new BoolDeserializerStrategy();
        yield return new ByteDeserializerStrategy();
        yield return new SByteDeserializerStrategy();
        yield return new CharDeserializerStrategy();
        yield return new DecimalDeserializerStrategy();
        yield return new DoubleDeserializerStrategy();
        yield return new FloatDeserializerStrategy();
        yield return new IntDeserializerStrategy();
        yield return new UIntDeserializerStrategy();
        yield return new NIntDeserializerStrategy();
        yield return new NUIntDeserializerStrategy();
        yield return new LongDeserializerStrategy();
        yield return new ULongDeserializerStrategy();
        yield return new ShortDeserializerStrategy();
        yield return new UShortDeserializerStrategy();
        yield return new StringDeserializerStrategy();
        yield return new ObjectDeserializerStrategy();
        yield return new Int128DeserializerStrategy();
        yield return new UInt128DeserializerStrategy();
    }
}

/// <summary>
/// <see cref="bool"/> deserializer strategy.
/// </summary>
public class BoolDeserializerStrategy : ArgumentDeserializerStrategy<bool>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(bool) || type == typeof(bool?);

    /// <inheritdoc/>
    public override bool Deserialize(Type type, string value)
    {
        if (value == "1")
        {
            return true;
        }
        else if (value == "0")
        {
            return false;
        }
        else
        {
            return bool.Parse(value);
        }
    }
}

/// <summary>
/// <see cref="byte"/> deserializer strategy.
/// </summary>
public class ByteDeserializerStrategy : ArgumentDeserializerStrategy<byte>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(byte) || type == typeof(byte?);

    /// <inheritdoc/>
    public override byte Deserialize(Type type, string value)
        => byte.Parse(value);
}

/// <summary>
/// <see cref="sbyte"/> deserializer strategy.
/// </summary>
public class SByteDeserializerStrategy : ArgumentDeserializerStrategy<sbyte>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(sbyte) || type == typeof(sbyte?);

    /// <inheritdoc/>
    public override sbyte Deserialize(Type type, string value)
        => sbyte.Parse(value);
}

/// <summary>
/// <see cref="char"/> deserializer strategy.
/// </summary>
public class CharDeserializerStrategy : ArgumentDeserializerStrategy<char>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(char) || type == typeof(char?);

    /// <inheritdoc/>
    public override char Deserialize(Type type, string value)
        => char.Parse(value);
}

/// <summary>
/// <see cref="decimal"/> deserializer strategy.
/// </summary>
public class DecimalDeserializerStrategy : ArgumentDeserializerStrategy<decimal>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(decimal) || type == typeof(decimal?);

    /// <inheritdoc/>
    public override decimal Deserialize(Type type, string value)
        => decimal.Parse(value);
}

/// <summary>
/// <see cref="double"/> deserializer strategy.
/// </summary>
public class DoubleDeserializerStrategy : ArgumentDeserializerStrategy<double>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(double) || type == typeof(double?);

    /// <inheritdoc/>
    public override double Deserialize(Type type, string value)
        => double.Parse(value);
}

/// <summary>
/// <see cref="float"/> deserializer strategy.
/// </summary>
public class FloatDeserializerStrategy : ArgumentDeserializerStrategy<float>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(float) || type == typeof(float?);

    /// <inheritdoc/>
    public override float Deserialize(Type type, string value)
        => float.Parse(value);
}

/// <summary>
/// <see cref="int"/> deserializer strategy.
/// </summary>
public class IntDeserializerStrategy : ArgumentDeserializerStrategy<int>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(int) || type == typeof(int?);

    /// <inheritdoc/>
    public override int Deserialize(Type type, string value)
        => int.Parse(value);
}

/// <summary>
/// <see cref="uint"/> deserializer strategy.
/// </summary>
public class UIntDeserializerStrategy : ArgumentDeserializerStrategy<uint>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(uint) || type == typeof(uint?);

    /// <inheritdoc/>
    public override uint Deserialize(Type type, string value)
        => uint.Parse(value);
}

/// <summary>
/// <see cref="nint"/> deserializer strategy.
/// </summary>
public class NIntDeserializerStrategy : ArgumentDeserializerStrategy<nint>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(nint) || type == typeof(nint?);

    /// <inheritdoc/>
    public override nint Deserialize(Type type, string value)
        => nint.Parse(value);
}

/// <summary>
/// <see cref="nuint"/> deserializer strategy.
/// </summary>
public class NUIntDeserializerStrategy : ArgumentDeserializerStrategy<nuint>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(nuint) || type == typeof(nuint?);

    /// <inheritdoc/>
    public override nuint Deserialize(Type type, string value)
        => nuint.Parse(value);
}

/// <summary>
/// <see cref="long"/> deserializer strategy.
/// </summary>
public class LongDeserializerStrategy : ArgumentDeserializerStrategy<long>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(long) || type == typeof(long?);

    /// <inheritdoc/>
    public override long Deserialize(Type type, string value)
        => long.Parse(value);
}

/// <summary>
/// <see cref="ulong"/> deserializer strategy.
/// </summary>
public class ULongDeserializerStrategy : ArgumentDeserializerStrategy<ulong>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(ulong) || type == typeof(ulong?);

    /// <inheritdoc/>
    public override ulong Deserialize(Type type, string value)
        => ulong.Parse(value);
}

/// <summary>
/// <see cref="short"/> deserializer strategy.
/// </summary>
public class ShortDeserializerStrategy : ArgumentDeserializerStrategy<short>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(short) || type == typeof(short?);

    /// <inheritdoc/>
    public override short Deserialize(Type type, string value)
        => short.Parse(value);
}

/// <summary>
/// <see cref="ushort"/> deserializer strategy.
/// </summary>
public class UShortDeserializerStrategy : ArgumentDeserializerStrategy<ushort>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(ushort) || type == typeof(ushort?);

    /// <inheritdoc/>
    public override ushort Deserialize(Type type, string value)
        => ushort.Parse(value);
}

/// <summary>
/// <see cref="string"/> deserializer strategy.
/// </summary>
public class StringDeserializerStrategy : ArgumentDeserializerStrategy<string>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(string);

    /// <inheritdoc/>
    public override string Deserialize(Type type, string value)
        => value;
}

/// <summary>
/// <see cref="object"/> deserializer strategy.
/// </summary>
public class ObjectDeserializerStrategy : ArgumentDeserializerStrategy<object>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(object);

    /// <inheritdoc/>
    public override object Deserialize(Type type, string value)
        => value;
}

/// <summary>
/// <see cref="Int128"/> deserializer strategy.
/// </summary>
public class Int128DeserializerStrategy : ArgumentDeserializerStrategy<Int128>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(Int128) || type == typeof(Int128?);

    /// <inheritdoc/>
    public override Int128 Deserialize(Type type, string value)
        => Int128.Parse(value);
}

/// <summary>
/// <see cref="UInt128"/> deserializer strategy.
/// </summary>
public class UInt128DeserializerStrategy : ArgumentDeserializerStrategy<UInt128>
{
    /// <inheritdoc/>
    public override bool MatchesType(Type type)
        => type == typeof(UInt128) || type == typeof(UInt128?);

    /// <inheritdoc/>
    public override UInt128 Deserialize(Type type, string value)
        => UInt128.Parse(value);
}
#pragma warning restore SA1402 // File may only contain a single type
