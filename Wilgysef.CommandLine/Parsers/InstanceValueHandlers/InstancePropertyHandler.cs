using System.Collections.Immutable;
using System.Reflection;

namespace Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

/// <summary>
/// Handler for setting instance properties.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
public class InstancePropertyHandler<T> : InstanceValueHandler<T>
{
    private readonly ImmutableDictionary<string, PropertyInfo> _propertyMap = typeof(T).GetProperties()
        .ToImmutableDictionary(p => p.Name);

    /// <inheritdoc/>
    public override bool HasValueName(string name)
        => _propertyMap.ContainsKey(name);

    /// <inheritdoc/>
    public override Type GetValueType(T instance, string name)
        => _propertyMap[name].PropertyType;

    /// <inheritdoc/>
    public override object? GetValue(T instance, string name)
        => _propertyMap[name].GetValue(instance);

    /// <inheritdoc/>
    public override void SetValue(T instance, string name, object? value)
        => _propertyMap[name].SetValue(instance, value);
}
