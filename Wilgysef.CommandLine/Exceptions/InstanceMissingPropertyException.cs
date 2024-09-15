namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument group instance is missing a property.
/// </summary>
public class InstanceMissingPropertyException : ArgumentParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceMissingPropertyException"/> class.
    /// </summary>
    /// <param name="argument">Argument.</param>
    /// <param name="argumentPosition">Argument position.</param>
    /// <param name="instanceName">Argument group instance name.</param>
    /// <param name="property">Property name.</param>
    public InstanceMissingPropertyException(string argument, int argumentPosition, string instanceName, string property)
        : base(argument, argumentPosition, $"The instance {instanceName} is missing the property {property}")
    {
        InstanceName = instanceName;
        Property = property;
    }

    /// <summary>
    /// Argument group instance name.
    /// </summary>
    public string InstanceName { get; }

    /// <summary>
    /// Property name.
    /// </summary>
    public string Property { get; }
}
