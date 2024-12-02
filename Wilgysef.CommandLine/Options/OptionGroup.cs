using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Options;

/// <summary>
/// Option group.
/// </summary>
public class OptionGroup : IOptionGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionGroup"/> class.
    /// </summary>
    /// <param name="name">Option group name.</param>
    /// <param name="min">Minimum number of expected options specified for the group, or <see langword="null"/> if all options are expected.</param>
    /// <param name="max">Maximum number of expected options specified for the group, or <see langword="null"/> if at least <paramref name="min"/> options are expected.</param>
    public OptionGroup(string name, int? min, int? max)
    {
        if (name.Length == 0)
        {
            throw new ArgumentException("Name length must be greater than 0", nameof(name));
        }

        Name = name;
        Min = min;
        Max = max;
    }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public int? Min { get; set; }

    /// <inheritdoc/>
    public int? Max { get; set; }

    /// <inheritdoc/>
    public bool IsMutuallyExclusive => Max == 1;

    /// <inheritdoc/>
    public bool IsRequired => Min == 1;

    /// <summary>
    /// Creates an <see cref="OptionGroup"/> where at least one option is expected to be specified.
    /// </summary>
    /// <param name="name">Option group name.</param>
    /// <returns>Option group.</returns>
    public static OptionGroup AtLeastOne(string name)
        => new(name, 1, null);

    /// <summary>
    /// Creates an <see cref="OptionGroup"/> where at most one option is expected to be specified.
    /// </summary>
    /// <param name="name">Option group name.</param>
    /// <returns>Option group.</returns>
    public static OptionGroup AtMostOne(string name)
        => new(name, 0, 1);

    /// <summary>
    /// Creates an <see cref="OptionGroup"/> where exactly one option is expected to be specified.
    /// </summary>
    /// <param name="name">Option group name.</param>
    /// <returns>Option group.</returns>
    public static OptionGroup ExactlyOne(string name)
        => new(name, 1, 1);

    /// <summary>
    /// Creates an <see cref="OptionGroup"/> where all options are expected to be specified.
    /// </summary>
    /// <param name="name">Option group name.</param>
    /// <returns>Option group.</returns>
    public static OptionGroup All(string name)
        => new(name, null, null);

    /// <inheritdoc/>
    public bool MatchesCount(int count, int optionsInGroup)
    {
        if (!Min.HasValue)
        {
            return count == optionsInGroup;
        }

        return Min.Value <= count && (!Max.HasValue || Max.Value >= count);
    }

    /// <inheritdoc/>
    public void Validate()
    {
        if (!Min.HasValue && Max.HasValue)
        {
            throw new InvalidOptionException(Name, "Minimum range must have a value if maximum does");
        }

        if (Min.HasValue && Max.HasValue && Min.Value > Max.Value)
        {
            throw new InvalidOptionException(Name, "Minimum cannot be greater than maximum");
        }
    }
}
