using Wilgysef.CommandLine.Commands;

namespace Wilgysef.CommandLine.Parsers;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Tokenized arguments.
/// </summary>
/// <param name="ArgumentGroups">Argument groups.</param>
public record TokenizedArguments(IReadOnlyList<ArgumentTokenGroup> ArgumentGroups);

/// <summary>
/// Group of <see cref="ArgumentToken"/>s separated by <see cref="ICommand"/>s.
/// </summary>
/// <param name="Arguments">Arguments.</param>
/// <param name="CommandMatch">Command match.</param>
public record ArgumentTokenGroup(IReadOnlyList<ArgumentToken> Arguments, CommandMatch? CommandMatch)
{
    /// <summary>
    /// Command of the group.
    /// </summary>
    public ICommand? Command => CommandMatch?.Command;
}

/// <summary>
/// Record for <see cref="ICommand"/> matches.
/// </summary>
/// <param name="Command">Command that matched.</param>
/// <param name="Argument">Argument matched.</param>
/// <param name="ArgumentPosition">Argument position matched at.</param>
public record CommandMatch(ICommand Command, string Argument, int ArgumentPosition);

/// <summary>
/// Argument token.
/// </summary>
public record ArgumentToken
{
    private ArgumentToken(
        Option? option,
        string arg,
        int argPosition,
        string argMatch,
        IReadOnlyList<string>? values)
    {
        Option = option;
        Argument = arg;
        ArgumentPosition = argPosition;
        ArgumentMatch = argMatch;
        Values = values;
    }

    /// <summary>
    /// Argument option.
    /// </summary>
    public Option? Option { get; }

    /// <summary>
    /// Argument value.
    /// </summary>
    public string Argument { get; }

    /// <summary>
    /// Position of argument in args list.
    /// </summary>
    public int ArgumentPosition { get; }

    /// <summary>
    /// The argument key name that matched the option, without prefixes.
    /// </summary>
    public string ArgumentMatch { get; }

    /// <summary>
    /// Option values.
    /// </summary>
    public IReadOnlyList<string>? Values { get; }

    /// <summary>
    /// Indicates whether the argument was given with a key-value pair.
    /// </summary>
    public bool ArgumentIsKeyValue => Argument != ArgumentMatch;

    /// <summary>
    /// Creates an unparsed argument.
    /// </summary>
    /// <param name="value">Unparsed argument value.</param>
    /// <param name="position">Argument position.</param>
    /// <returns>Unparsed argument.</returns>
    public static ArgumentToken Unparsed(string value, int position)
    {
        return new ArgumentToken(null, value, position, value, new[] { value });
    }

    /// <summary>
    /// Creates an option argument with no value.
    /// </summary>
    /// <param name="option">Argument option.</param>
    /// <param name="arg">Argument value.</param>
    /// <param name="position">Argument position.</param>
    /// <param name="argMatch">Argument value key name without option prefixes.</param>
    /// <returns>Argument option with no value.</returns>
    public static ArgumentToken NoValue(Option option, string arg, int position, string argMatch)
    {
        return new ArgumentToken(option, arg, position, argMatch, null);
    }

    /// <summary>
    /// Creates an option argument with one value.
    /// </summary>
    /// <param name="option">Argument option.</param>
    /// <param name="arg">Argument value.</param>
    /// <param name="position">Argument position.</param>
    /// <param name="argMatch">Argument value key name without option prefixes.</param>
    /// <param name="value">Value specified.</param>
    /// <returns>Argument option with one value.</returns>
    public static ArgumentToken OneValue(Option option, string arg, int position, string argMatch, string value)
    {
        return new ArgumentToken(option, arg, position, argMatch, new[] { value });
    }

    /// <summary>
    /// Creates an option argument.
    /// </summary>
    /// <param name="option">Argument option.</param>
    /// <param name="arg">Argument value.</param>
    /// <param name="position">Argument position.</param>
    /// <param name="argMatch">Argument value key name without option prefixes.</param>
    /// <param name="values">Values specified.</param>
    /// <returns>Argument option with values.</returns>
    public static ArgumentToken WithValues(
        Option option,
        string arg,
        int position,
        string argMatch,
        IReadOnlyList<string> values)
    {
        return new ArgumentToken(option, arg, position, argMatch, values.Count > 0 ? values : null);
    }
}
#pragma warning restore SA1402 // File may only contain a single type
