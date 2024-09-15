using System.Collections.Immutable;
using System.Reflection;
using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public class InstanceAnonymousValueHandler<T> : InstanceValueHandler<T>
{
    private static readonly string[] _backingFieldFormats = { "<{0}>i__Field", "<{0}>" };

    private readonly FieldInfo[] _fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
    private readonly ImmutableDictionary<string, PropertyInfo> _propertyMap = typeof(T).GetProperties()
        .ToImmutableDictionary(p => p.Name);

    public override bool HasValueName(string name)
        => _propertyMap.ContainsKey(name);

    public override Type GetValueType(T instance, string name)
        => _propertyMap[name].PropertyType;

    public override object? GetValue(T instance, string name)
        => _propertyMap[name].GetValue(instance);

    // https://stackoverflow.com/a/30242237
    public override void SetValue(T instance, string name, object? value)
    {
        var backingFieldNames = _backingFieldFormats.Select(x => string.Format(x, name))
            .ToList();

        var field = _fields.FirstOrDefault(f => backingFieldNames.Contains(f.Name))
            ?? throw new ArgumentException($"Could not find anonymous object backing field for {name}");

        field.SetValue(instance, value);
    }
}
