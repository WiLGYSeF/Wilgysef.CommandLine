namespace Wilgysef.CommandLine.Commands;

/// <summary>
/// Command factory.
/// </summary>
/// <typeparam name="T">Command type.</typeparam>
public interface ICommandFactory<T> : ICommandFactory
    where T : ICommand
{
    /// <summary>
    /// Creates a command for execution.
    /// </summary>
    /// <returns>Command.</returns>
    new T Create();
}

/// <summary>
/// Command factory.
/// </summary>
public interface ICommandFactory : ICommandConfiguration
{
    /// <summary>
    /// Creates a command for execution.
    /// </summary>
    /// <returns>Command.</returns>
    ICommand Create();
}
