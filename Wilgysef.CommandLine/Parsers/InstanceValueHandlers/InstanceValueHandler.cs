namespace Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

/// <summary>
/// Handler for getting and setting instance values.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
public abstract class InstanceValueHandler<T> : IInstanceValueHandler<T>
{
    /// <inheritdoc/>
    public abstract bool HasValueName(string name);

    /// <inheritdoc/>
    public abstract Type GetValueType(T instance, string name);

    /// <inheritdoc/>
    public abstract object? GetValue(T instance, string name);

    /// <inheritdoc/>
    public abstract void SetValue(T instance, string name, object? value);

    /// <inheritdoc/>
    Type IInstanceValueHandler.GetValueType(object instance, string name)
        => GetValueType((T)instance, name);

    /// <inheritdoc/>
    object? IInstanceValueHandler.GetValue(object instance, string name)
        => GetValue((T)instance, name);

    /// <inheritdoc/>
    void IInstanceValueHandler.SetValue(object instance, string name, object? value)
        => SetValue((T)instance, name, value);
}
