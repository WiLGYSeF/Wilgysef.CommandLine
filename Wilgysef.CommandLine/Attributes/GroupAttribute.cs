namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to set option groups.
/// </summary>
/// <param name="groups">Option group names.</param>
[AttributeUsage(AttributeTargets.Property)]
public class GroupAttribute(params string[] groups) : Attribute
{
    /// <summary>
    /// Option group names.
    /// </summary>
    public IReadOnlyList<string> OptionGroups { get; } = groups;
}
