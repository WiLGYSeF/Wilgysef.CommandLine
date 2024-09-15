namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Parser options.
/// </summary>
public interface IParserOptions
{
    /// <summary>
    /// Argument to specify subsequent arguments are literals.
    /// </summary>
    string? ArgumentLiteralSeparator { get; set; }

    /// <summary>
    /// Throw if an argument has an unexpected key-value.
    /// </summary>
    bool? ThrowOnArgumentUnexpectedKeyValue { get; set; }

    /// <summary>
    /// Throw if an unknown option is encountered.
    /// </summary>
    bool? ThrowOnUnknownOptions { get; set; }

    /// <summary>
    /// Ignore unknown options.
    /// </summary>
    bool? IgnoreUnknownOptions { get; set; }

    /// <summary>
    /// Throw if an unknown short option is encountered.
    /// </summary>
    bool? ThrowOnUnknownShortOptions { get; set; }
}
