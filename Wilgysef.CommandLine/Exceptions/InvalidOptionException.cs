namespace Wilgysef.CommandLine.Exceptions;

public class InvalidOptionException : Exception
{
    public string? OptionName { get; }

    public InvalidOptionException(string optionName, string? message, Exception? innerException = null)
        : this(message, innerException)
    {
        OptionName = optionName;
    }

    public InvalidOptionException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
