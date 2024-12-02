using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Values;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Properties for argument registration.
/// </summary>
public interface IArgumentRegistrationProperties
{
    /// <summary>
    /// Options.
    /// </summary>
    ICollection<IOption> Options { get; set; }

    /// <summary>
    /// Option groups.
    /// </summary>
    ICollection<IOptionGroup> OptionGroups { get; set; }

    /// <summary>
    /// Values.
    /// </summary>
    ICollection<IValue> Values { get; set; }

    /// <summary>
    /// Commands.
    /// </summary>
    ICollection<ICommandConfiguration> Commands { get; set; }
}
