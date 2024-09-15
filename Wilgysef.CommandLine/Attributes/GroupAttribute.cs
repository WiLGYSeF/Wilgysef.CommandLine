namespace Wilgysef.CommandLine.Attributes;

/// <summary>
/// Attribute to set option groups.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class GroupAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupAttribute"/> class.
    /// </summary>
    /// <param name="groups">Option group names.</param>
    public GroupAttribute(params string[] groups)
    {
        OptionGroups = groups;
    }

    /// <summary>
    /// Option group names.
    /// </summary>
    public IReadOnlyList<string> OptionGroups { get; }
}
