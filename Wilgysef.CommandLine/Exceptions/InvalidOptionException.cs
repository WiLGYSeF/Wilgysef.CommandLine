namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an invalid option is configured.
/// </summary>
public class InvalidOptionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOptionException"/> class.
    /// </summary>
    /// <param name="optionName">Option name.</param>
    /// <param name="message">Message.</param>
    public InvalidOptionException(string optionName, string? message)
        : base(message, null)
    {
        OptionName = optionName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOptionException"/> class.
    /// </summary>
    /// <param name="optionName">Option name.</param>
    /// <param name="message">Message.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidOptionException(string optionName, string? message, Exception? innerException)
        : base(message, innerException)
    {
        OptionName = optionName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOptionException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    public InvalidOptionException(string? message)
        : base(message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOptionException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidOptionException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Option name.
    /// </summary>
    public string? OptionName { get; }
}
