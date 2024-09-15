using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if the option group's expected option count and the specified option count mismatch.
/// </summary>
public class OptionGroupOptionMismatchException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionGroupOptionMismatchException"/> class.
    /// </summary>
    /// <param name="optionGroup">Option group.</param>
    /// <param name="options">Options in option group.</param>
    /// <param name="argumentTokens">Argument tokens of options.</param>
    /// <param name="message">Message.</param>
    /// <param name="innerException">Inner exception.</param>
    public OptionGroupOptionMismatchException(
        OptionGroup? optionGroup,
        ICollection<Option> options,
        ICollection<ArgumentToken> argumentTokens,
        string message,
        Exception? innerException = null)
        : base(
            argumentTokens.FirstOrDefault()?.Argument ?? options.First().GetOptionArgument(),
            argumentTokens.FirstOrDefault()?.ArgumentPosition ?? -1,
            message,
            innerException)
    {
        OptionGroup = optionGroup;
        Options = options;
        ArgumentTokens = argumentTokens;
    }

    /// <summary>
    /// Option group.
    /// </summary>
    public OptionGroup? OptionGroup { get; }

    /// <summary>
    /// Options in option group.
    /// </summary>
    public ICollection<Option> Options { get; }

    /// <summary>
    /// Argument tokens of options.
    /// </summary>
    public ICollection<ArgumentToken> ArgumentTokens { get; }

    /// <summary>
    /// Indicates if there are required options missing.
    /// </summary>
    public bool MissingRequiredOption { get; private set; }

    /// <summary>
    /// Indicates if options are mutually exclusive.
    /// </summary>
    public bool MutuallyExclusiveOptions { get; private set; }

    /// <summary>
    /// A required option is missing.
    /// </summary>
    /// <param name="option">Option.</param>
    /// <returns><see cref="OptionGroupOptionMismatchException"/>.</returns>
    public static OptionGroupOptionMismatchException RequiredOptionMissing(Option option)
    {
        var message = $"The option \"{option.GetOptionArgument()}\" is required";
        return new OptionGroupOptionMismatchException(null, [option], Array.Empty<ArgumentToken>(), message)
        {
            MissingRequiredOption = true,
        };
    }

    /// <summary>
    /// Required options are missing.
    /// </summary>
    /// <param name="group">Option group.</param>
    /// <param name="options">Options in option group.</param>
    /// <returns><see cref="OptionGroupOptionMismatchException"/>.</returns>
    public static OptionGroupOptionMismatchException RequiredOptionMissing(
        OptionGroup group,
        ICollection<Option> options)
    {
        var message = $"At least one of these options are required: {string.Join(", ", options.Select(o => o.GetOptionArgument()))}";
        return new OptionGroupOptionMismatchException(group, options, Array.Empty<ArgumentToken>(), message)
        {
            MissingRequiredOption = true,
        };
    }

    /// <summary>
    /// Options are mutually exclusive.
    /// </summary>
    /// <param name="group">Option group.</param>
    /// <param name="options">Options in option group.</param>
    /// <param name="argTokens">Argument tokens of options.</param>
    /// <returns><see cref="OptionGroupOptionMismatchException"/>.</returns>
    public static OptionGroupOptionMismatchException MutuallyExclusiveOption(
        OptionGroup group,
        ICollection<Option> options,
        ICollection<ArgumentToken> argTokens)
    {
        var first = argTokens.ElementAt(0);
        var second = argTokens.ElementAt(1);

        var message = $"The arguments \"{first.Argument}\" at position {first.ArgumentPosition} and \"{second.Argument}\" at position {second.ArgumentPosition} are mutually exclusive";
        return new OptionGroupOptionMismatchException(group, options, argTokens, message)
        {
            MutuallyExclusiveOptions = true,
        };
    }

    /// <summary>
    /// Option group expected count does not match the specified option count.
    /// </summary>
    /// <param name="group">Option group.</param>
    /// <param name="options">Options in option group.</param>
    /// <param name="argTokens">Argument tokens of options.</param>
    /// <returns><see cref="OptionGroupOptionMismatchException"/>.</returns>
    public static OptionGroupOptionMismatchException OptionMismatch(
        OptionGroup group,
        ICollection<Option> options,
        ICollection<ArgumentToken> argTokens)
    {
        var optionStrs = string.Join(", ", options.Select(o => o.GetOptionArgument()));
        var message = group.Min != group.Max
            ? $"Expected between {group.Min} and {group.Max} of these options to be specified: {optionStrs}"
            : $"Expected {group.Min} options to be specified: {optionStrs}";
        return new OptionGroupOptionMismatchException(group, options, argTokens, message);
    }
}
