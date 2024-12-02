using Wilgysef.CommandLine.Options;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if a duplicate <see cref="IOption"/> is encountered.
/// </summary>
/// <param name="optionName">Option name.</param>
/// <param name="otherOptionName">Other option name.</param>
/// <param name="message">Message.</param>
/// <param name="innerException">Inner exception.</param>
public class DuplicateOptionException(
    string optionName,
    string otherOptionName,
    string? message,
    Exception? innerException = null)
    : InvalidOptionException(optionName, message, innerException)
{
    /// <summary>
    /// Other option name.
    /// </summary>
    public string OtherOptionName { get; } = otherOptionName;

    /// <summary>
    /// Creates a <see cref="DuplicateOptionException"/> with an option name.
    /// </summary>
    /// <param name="name">Option name.</param>
    /// <returns>Exception.</returns>
    public static DuplicateOptionException Name(string name)
        => new(name, name, $"The option \"{name}\" has already been declared");

    /// <summary>
    /// Creates a <see cref="DuplicateOptionException"/> with a command name.
    /// </summary>
    /// <param name="name">Command name.</param>
    /// <returns>Exception.</returns>
    public static DuplicateOptionException CommandName(string name)
        => new(name, name, $"The command \"{name}\" has already been declared");

    /// <summary>
    /// Creates a <see cref="DuplicateOptionException"/> for a short option.
    /// </summary>
    /// <param name="optionName">Option name.</param>
    /// <param name="otherOptionName">Other option name.</param>
    /// <param name="shortOpt">Short option.</param>
    /// <returns>Exception.</returns>
    public static DuplicateOptionException ShortOption(string optionName, string otherOptionName, char shortOpt)
        => new(optionName, otherOptionName, $"The short option '{shortOpt}' in \"{otherOptionName}\" was already declared in \"{optionName}\"");

    /// <summary>
    /// Creates a <see cref="DuplicateOptionException"/> for a long option.
    /// </summary>
    /// <param name="optionName">Option name.</param>
    /// <param name="otherOptionName">Other option name.</param>
    /// <param name="longOpt">Long option.</param>
    /// <returns>Exception.</returns>
    public static DuplicateOptionException LongOption(string optionName, string otherOptionName, string longOpt)
        => new(optionName, otherOptionName, $"The option '{longOpt}' in \"{otherOptionName}\" was already declared in \"{optionName}\"");
}
