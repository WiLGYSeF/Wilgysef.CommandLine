using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to designate a property as an <see cref="Option"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute : Attribute
{
    /// <summary>
    /// Option description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the option should be hidden from help providers.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Indicates if the option is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Indicates if the option should only be provided once.
    /// </summary>
    public bool Unique { get; set; }
}
