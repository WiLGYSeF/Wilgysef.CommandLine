namespace Wilgysef.CommandLine.Exceptions;

/// <summary>
/// Thrown if an argument group instance is missing a property.
/// </summary>
/// <param name="argument">Argument.</param>
/// <param name="argumentPosition">Argument position.</param>
/// <param name="instanceName">Argument group instance name.</param>
/// <param name="property">Property name.</param>
public class InstanceMissingPropertyException(
    string argument,
    int argumentPosition,
    string instanceName,
    string property)
    : ArgumentParseException(argument, argumentPosition, $"The instance {instanceName} is missing the property {property}")
{
    /// <summary>
    /// Argument group instance name.
    /// </summary>
    public string InstanceName { get; } = instanceName;

    /// <summary>
    /// Property name.
    /// </summary>
    public string Property { get; } = property;
}
