namespace Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

/// <summary>
/// Interface for getting and setting instance values.
/// </summary>
/// <typeparam name="T">Instance type.</typeparam>
public interface IInstanceValueHandler<T> : IInstanceValueHandler
{
    /// <summary>
    /// Gets the value type on the <paramref name="instance"/> by name.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <returns>Value type.</returns>
    Type GetValueType(T instance, string name);

    /// <summary>
    /// Gets the value of <paramref name="name"/> on the <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <returns>Value.</returns>
    object? GetValue(T instance, string name);

    /// <summary>
    /// Sets the value of <paramref name="name"/> on the <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <param name="value">Value value.</param>
    void SetValue(T instance, string name, object? value);
}

/// <summary>
/// Interface for getting and setting instance values.
/// </summary>
public interface IInstanceValueHandler
{
    /// <summary>
    /// Checks if the value name <paramref name="name"/> exists on the instance type.
    /// </summary>
    /// <param name="name">Value name.</param>
    /// <returns><see langword="true"/> if the value name exists on the instance type, otherwise <see langword="false"/>.</returns>
    bool HasValueName(string name);

    /// <summary>
    /// Gets the value type on the <paramref name="instance"/> by name.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <returns>Value type.</returns>
    Type GetValueType(object instance, string name);

    /// <summary>
    /// Gets the value of <paramref name="name"/> on the <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <returns>Value.</returns>
    object? GetValue(object instance, string name);

    /// <summary>
    /// Sets the value of <paramref name="name"/> on the <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="name">Value name.</param>
    /// <param name="value">Value value.</param>
    void SetValue(object instance, string name, object? value);
}
