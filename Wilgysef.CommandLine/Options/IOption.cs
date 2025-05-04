using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Generators;

namespace Wilgysef.CommandLine.Options;

/// <summary>
/// Used to parse argument options.
/// </summary>
[GenerateFluentPattern]
public interface IOption : IOptionProperties
{
    /// <summary>
    /// Option name.
    /// </summary>
    /// <remarks>
    /// Used to determine the instance property to set when parsing.
    /// </remarks>
    string Name { get; set; }

    /// <summary>
    /// Option description.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Indicates if the option should be hidden from help providers.
    /// </summary>
    bool Hidden { get; set; }

    /// <summary>
    /// Value names to display in help providers.
    /// </summary>
    ICollection<string>? ValueNames { get; set; }

    /// <summary>
    /// Option short names.
    /// </summary>
    ICollection<char>? ShortNames { get; set; }

    /// <summary>
    /// Option long names.
    /// </summary>
    ICollection<string>? LongNames { get; set; }

    /// <summary>
    /// Number of values the option may have.
    /// </summary>
    ValueRange? ValueCountRange { get; set; }

    /// <summary>
    /// Indicates if the option is a boolean switch value.
    /// </summary>
    bool Switch { get; set; }

    /// <summary>
    /// Switch negation long prefix used to configure a negatable option.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--no-cache</c>, <c>"no-"</c> is the negated switch long prefix.
    /// </remarks>
    string? SwitchNegateLongPrefix { get; set; }

    /// <summary>
    /// Switch negation short names.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P</c>, <c>P</c> is the negated short name.
    /// </remarks>
    ICollection<char>? SwitchNegateShortNames { get; set; }

    /// <summary>
    /// Indicates if the option is a counter value.
    /// </summary>
    bool Counter { get; set; }

    /// <summary>
    /// Indicates if the option is required.
    /// </summary>
    bool Required { get; set; }

    /// <summary>
    /// Name of groups the option belongs to.
    /// </summary>
    ICollection<string>? GroupNames { get; set; }

    /// <summary>
    /// Indicates if the option should only be provided once.
    /// </summary>
    bool Unique { get; set; }

    /// <summary>
    /// Indicates if the option has short names.
    /// </summary>
    bool HasShortNames { get; }

    /// <summary>
    /// Indicates if the option has long names.
    /// </summary>
    bool HasLongNames { get; }

    /// <summary>
    /// Indicates if the option can have values.
    /// </summary>
    bool CanHaveValues { get; }

    /// <summary>
    /// Checks if <paramref name="ch"/> matches the option.
    /// </summary>
    /// <param name="ch">Argument name, without prefixes or key-values.</param>
    /// <param name="switchNegated">Indicates if the argument is a switch-negation.</param>
    /// <returns><see langword="true"/> if <paramref name="ch"/> matches the option, otherwise <see langword="false"/>.</returns>
    bool MatchesShortName(char ch, out bool switchNegated);

    /// <summary>
    /// Checks if <paramref name="argName"/> matches the option.
    /// </summary>
    /// <param name="argName">Argument name, without prefixes or key-values.</param>
    /// <param name="switchNegated">Indicates if the argument is a switch-negation.</param>
    /// <param name="ignoreCaseDefault">Whether to ignore case-sensitivity if <see cref="IOptionProperties.LongNameCaseInsensitive"/> is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="argName"/> matches the option, otherwise <see langword="false"/>.</returns>
    bool MatchesLongName(string argName, out bool switchNegated, bool ignoreCaseDefault = false);

    /// <summary>
    /// Gets the option argument.
    /// </summary>
    /// <param name="shortNamePrefixDefault">Short name prefix default if <see cref="IOptionProperties.ShortNamePrefix"/> is <see langword="null"/>.</param>
    /// <param name="longNamePrefixDefault">Long name prefix default if <see cref="IOptionProperties.LongNamePrefix"/> is <see langword="null"/>.</param>
    /// <returns>Option argument.</returns>
    string GetOptionArgument(string shortNamePrefixDefault = "-", string longNamePrefixDefault = "--");

    /// <summary>
    /// Validates the option.
    /// </summary>
    /// <exception cref="InvalidOptionException">Thrown if the option is invalid.</exception>
    void Validate();
}
