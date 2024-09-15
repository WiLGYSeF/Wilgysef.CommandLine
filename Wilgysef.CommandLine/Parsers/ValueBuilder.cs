using System.Reflection;
using Wilgysef.CommandLine.Attributes;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Builds <see cref="Value"/>s from objects with <see cref="ValueAttribute"/>s.
/// </summary>
public class ValueBuilder
{
    /// <summary>
    /// Creates <see cref="Value"/>s from an object type <typeparamref name="T"/> with <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <returns>Values.</returns>
    public static List<Value> BuildValues<T>()
        => BuildValues(typeof(T));

    /// <summary>
    /// Creates <see cref="Value"/>s from a <paramref name="type"/> with <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <param name="type">Object type.</param>
    /// <returns>Values.</returns>
    public static List<Value> BuildValues(Type type)
    {
        var values = new List<Value>();

        foreach (var prop in type.GetProperties())
        {
            var valueAttr = prop.GetCustomAttribute<ValueAttribute>();
            if (valueAttr == null)
            {
                continue;
            }

            values.Add(new Value(prop.Name, new ValueRange(valueAttr.Min, valueAttr.Max))
            {
                Description = valueAttr.Description,
                ValueName = valueAttr.ValueName,
            });
        }

        return values;
    }
}
