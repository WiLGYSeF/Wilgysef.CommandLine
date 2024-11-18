using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;
using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command-line command.
/// </summary>
/// <typeparam name="T">Command options type.</typeparam>
public abstract class Command<T> : Command, ICommand<T>
    where T : class
{
    /// <inheritdoc/>
    public virtual T OptionsFactory()
    {
        return Activator.CreateInstance<T>();
    }

    /// <inheritdoc/>
    public abstract void Execute(ICommandExecutionContext context, T options);

    /// <inheritdoc/>
    public override void Execute(ICommandExecutionContext context)
        => Execute(context, null!);
}

/// <summary>
/// Command-line command.
/// </summary>
public abstract class Command : ICommand
{
    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <inheritdoc/>
    public ICollection<string>? Aliases { get; set; }

    /// <inheritdoc/>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public bool Hidden { get; set; }

    /// <inheritdoc/>
    public bool CaseInsensitiveNameMatch { get; set; }

    /// <inheritdoc/>
    public ICollection<Option> Options { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<OptionGroup> OptionGroups { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<Value> Values { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<ICommand> Commands { get; set; } = [];

    /// <inheritdoc/>
    public string? ShortNamePrefix { get; set; }

    /// <inheritdoc/>
    public string? LongNamePrefix { get; set; }

    /// <inheritdoc/>
    public ICollection<string>? KeyValueSeparators { get; set; } = [];

    /// <inheritdoc/>
    public bool? ShortNameImmediateValue { get; set; }

    /// <inheritdoc/>
    public bool? LongNameCaseInsensitive { get; set; }

    /// <inheritdoc/>
    public bool? KeepFirstValue { get; set; }

    /// <inheritdoc/>
    public string? ArgumentLiteralSeparator { get; set; }

    /// <inheritdoc/>
    public bool? ThrowOnArgumentUnexpectedKeyValue { get; set; }

    /// <inheritdoc/>
    public bool? ThrowOnUnknownOptions { get; set; }

    /// <inheritdoc/>
    public bool? IgnoreUnknownOptions { get; set; }

    /// <inheritdoc/>
    public bool? ThrowOnUnknownShortOptions { get; set; }

    /// <inheritdoc/>
    public ICollection<IArgumentDeserializerStrategy> Deserializers { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<IArgumentValueListDeserializerStrategy> ListDeserializers { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<IArgumentValueAggregator> ValueAggregators { get; set; } = [];

    /// <inheritdoc/>
    public IInstanceValueHandler? InstanceValueHandler { get; set; }

    /// <inheritdoc/>
    public bool? ThrowOnMissingProperty { get; set; }

    /// <inheritdoc/>
    public bool? ThrowOnTooManyValues { get; set; }

    /// <inheritdoc/>
    public abstract void Execute(ICommandExecutionContext context);

    /// <inheritdoc/>
    public virtual bool Matches(string arg)
    {
        var comparison = CaseInsensitiveNameMatch
            ? StringComparison.CurrentCultureIgnoreCase
            : StringComparison.CurrentCulture;
        return Name.Equals(arg, comparison)
            || (Aliases != null && Aliases.Any(a => a.Equals(arg, comparison)));
    }
}
