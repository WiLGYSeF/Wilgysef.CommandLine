using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine.Options;

/// <summary>
/// Option group.
/// </summary>
public interface IOptionGroup
{
    /// <summary>
    /// Option group name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Minimum number of expected options specified for the group, or <see langword="null"/> if all options are expected.
    /// </summary>
    int? Min { get; set; }

    /// <summary>
    /// Maximum number of expected options specified for the group, or <see langword="null"/> if at least <see name="Min"/> options are expected.
    /// </summary>
    int? Max { get; set; }

    /// <summary>
    /// Indicates if the option group is mutually exclusive.
    /// </summary>
    bool IsMutuallyExclusive { get; }

    /// <summary>
    /// Indicates if the option group is required.
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    /// Checks if the <paramref name="count"/> of options specified is valid when there are <paramref name="optionsInGroup"/> number of options in the group.
    /// </summary>
    /// <param name="count">Options specified count.</param>
    /// <param name="optionsInGroup">Number of options in group.</param>
    /// <returns><see langword="true"/> if the count matches the number of options in the group, otherwise <see langword="false"/>.</returns>
    bool MatchesCount(int count, int optionsInGroup);

    /// <summary>
    /// Validates the option group.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the option group is not valid.</exception>
    void Validate();
}
