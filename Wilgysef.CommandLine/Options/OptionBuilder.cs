using System.Reflection;
using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Options;

/// <summary>
/// Builds <see cref="Option"/>s from objects with <see cref="OptionAttribute"/>s.
/// </summary>
public static class OptionBuilder
{
    /// <summary>
    /// Creates <see cref="Option"/>s from an object type <typeparamref name="T"/> with <see cref="OptionAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <returns>Options.</returns>
    public static List<Option> BuildOptions<T>()
        => BuildOptions(typeof(T));

    /// <summary>
    /// Creates <see cref="Option"/>s from an object type with <see cref="OptionAttribute"/>s.
    /// </summary>
    /// <param name="type">Object type.</param>
    /// <returns>Options.</returns>
    public static List<Option> BuildOptions(Type type)
    {
        var options = new List<Option>();

        foreach (var prop in type.GetProperties())
        {
            var optionAttr = prop.GetCustomAttribute<OptionAttribute>();
            if (optionAttr == null)
            {
                continue;
            }

            var shortNameAttr = prop.GetCustomAttribute<ShortNameAttribute>();
            var longNameAttr = prop.GetCustomAttribute<LongNameAttribute>();
            var switchAttr = prop.GetCustomAttribute<SwitchAttribute>();
            var counterAttr = prop.GetCustomAttribute<CounterAttribute>();
            var valueCountRangeAttr = prop.GetCustomAttribute<ValueCountAttribute>();
            var groupAttr = prop.GetCustomAttribute<GroupAttribute>();

            options.Add(new Option(prop.Name)
            {
                Description = optionAttr.Description,
                Hidden = optionAttr.Hidden,
                ValueNames = valueCountRangeAttr?.ValueNames.ToList(),
                ShortNames = shortNameAttr?.ShortNames.ToList(),
                LongNames = longNameAttr?.LongNames.ToList(),
                ShortNamePrefix = shortNameAttr?.ShortNamePrefix,
                LongNamePrefix = longNameAttr?.LongNamePrefix,
                KeyValueSeparators = valueCountRangeAttr?.KeyValueSeparator != null
                    ? [valueCountRangeAttr.KeyValueSeparator]
                    : null,
                ValueCountRange = valueCountRangeAttr != null
                    ? new ValueRange(valueCountRangeAttr.Min, valueCountRangeAttr.Max)
                    : null,
                Switch = switchAttr != null,
                SwitchNegateLongPrefix = switchAttr?.SwitchNegateLongPrefix,
                SwitchNegateShortNames = switchAttr != null
                    ? new List<char> { switchAttr.SwitchNegateShortName, }
                    : null,
                Counter = counterAttr != null,
                Required = optionAttr.Required,
                GroupNames = groupAttr?.OptionGroups.ToList(),
                Unique = optionAttr.Unique,
                ShortNameImmediateValue = shortNameAttr?.ShortNameImmediateValue,
                LongNameCaseInsensitive = longNameAttr?.LongNameCaseInsensitive,
                KeepFirstValue = valueCountRangeAttr?.KeepFirstValue,
            });
        }

        return options;
    }
}
