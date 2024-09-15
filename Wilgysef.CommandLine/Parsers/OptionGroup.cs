using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Option group.
/// </summary>
public class OptionGroup
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

    /// <summary>
    /// Option group name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Minimum number of expected options specified for the group, or <see langword="null"/> if all options are expected.
    /// </summary>
    public int? Min { get; set; }

    /// <summary>
    /// Maximum number of expected options specified for the group, or <see langword="null"/> if at least <see name="Min"/> options are expected.
    /// </summary>
    public int? Max { get; set; }

    /// <summary>
    /// Indicates if the option group is mutually exclusive.
    /// </summary>
    public bool IsMutuallyExclusive => Max == 1;

    /// <summary>
    /// Indicates if the option group is required.
    /// </summary>
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

    /// <summary>
    /// Checks if the <paramref name="count"/> of options specified is valid when there are <paramref name="optionsInGroup"/> number of options in the group.
    /// </summary>
    /// <param name="count">Options specified count.</param>
    /// <param name="optionsInGroup">Number of options in group.</param>
    /// <returns><see langword="true"/> if the count matches the number of options in the group, otherwise <see langword="false"/>.</returns>
    public bool MatchesCount(int count, int optionsInGroup)
    {
        if (!Min.HasValue)
        {
            return count == optionsInGroup;
        }

        return Min.Value <= count && (!Max.HasValue || Max.Value >= count);
    }

    /// <summary>
    /// Validates the option group.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the option group is not valid.</exception>
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
