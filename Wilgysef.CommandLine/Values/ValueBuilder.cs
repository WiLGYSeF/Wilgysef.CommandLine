using System.Reflection;
using Wilgysef.CommandLine.Attributes;

namespace Wilgysef.CommandLine.Values;

/// <summary>
/// Builder for <see cref="Value"/>s.
/// </summary>
public class ValueBuilder
{
    /// <summary>
    /// Value name.
    /// </summary>
    /// <remarks>
    /// Used to determine the instance property to set when parsing.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Value description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Value display name.
    /// </summary>
    public string? ValueName { get; set; }

    /// <summary>
    /// Argument position start index.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// Argument position end index.
    /// </summary>
    public int? EndIndex { get; set; }

    public ValueBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public ValueBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public ValueBuilder WithValueName(string valueName)
    {
        ValueName = valueName;
        return this;
    }

    public ValueBuilder Single(int index)
    {
        StartIndex = index;
        EndIndex = index;
        return this;
    }

    public ValueBuilder StartingAt(int index)
    {
        StartIndex = index;
        EndIndex = index;
        return this;
    }

    public ValueBuilder AtForNext(int index, int count)
    {
        StartIndex = index;
        EndIndex = index + count;
        return this;
    }

    public ValueBuilder All()
    {
        StartIndex = 0;
        EndIndex = null;
        return this;
    }

    public Value Build()
    {
        if (Name == null)
        {
            throw new Exception("Name must be set");
        }

        return new Value(Name, StartIndex, EndIndex)
        {
            Description = Description,
            ValueName = ValueName
        };
    }

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

            values.Add(new Value(prop.Name, valueAttr.StartIndex, valueAttr.EndIndex)
            {
                Description = valueAttr.Description,
                ValueName = valueAttr.ValueName,
            });
        }

        return values;
    }
}
