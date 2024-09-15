using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command interface.
/// </summary>
/// <typeparam name="T">Command settings type.</typeparam>
public interface ICommand<T> : ICommand
    where T : class
{
    /// <summary>
    /// Factory for the command settings.
    /// </summary>
    /// <returns>Settings instance.</returns>
    T SettingsFactory();

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    /// <param name="settings">Command settings.</param>
    void Execute(CommandExecutionContext context, T settings);
}

/// <summary>
/// Command interface.
/// </summary>
public interface ICommand
    : IArgumentRegistrationProperties,
    IOptionProperties,
    IParserOptions,
    IDeserializationOptions,
    IInstanceHandlerOptions
{
    /// <summary>
    /// Command name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Command aliases.
    /// </summary>
    ICollection<string>? Aliases { get; set; }

    /// <summary>
    /// Command description.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Indicates if the option should be hidden from help providers.
    /// </summary>
    bool Hidden { get; set; }

    /// <summary>
    /// Whether the command name should match case-insensitive.
    /// </summary>
    bool CaseInsensitiveNameMatch { get; set; }

    /// <summary>
    /// Checks if <paramref name="arg"/> matches the command.
    /// </summary>
    /// <param name="arg">Argument.</param>
    /// <returns><see langword="true"/> if the argument matches, otherwise <see langword="false"/>.</returns>
    bool Matches(string arg);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">Execution context.</param>
    void Execute(CommandExecutionContext context);
}
