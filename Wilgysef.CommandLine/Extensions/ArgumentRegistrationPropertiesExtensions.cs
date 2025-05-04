using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Parsers;
using Wilgysef.CommandLine.Values;

namespace Wilgysef.CommandLine.Extensions;

/// <summary>
/// <see cref="IArgumentRegistrationProperties"/> extension methods.
/// </summary>
public static class ArgumentRegistrationPropertiesExtensions
{
    /// <summary>
    /// Configures <paramref name="registration"/> with options and values from <typeparamref name="T"/> specified by <see cref="OptionAttribute"/>s and <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithOptionsValuesFrom<T>(this IArgumentRegistrationProperties registration)
        where T : class
        => WithOptionsValuesFrom(registration, typeof(T));

    /// <summary>
    /// Configures <paramref name="registration"/> with options and values from <paramref name="type"/> specified by <see cref="OptionAttribute"/>s and <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="type">Object type.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithOptionsValuesFrom(
        this IArgumentRegistrationProperties registration,
        Type type)
    {
        foreach (var option in OptionBuilder.BuildOptions(type))
        {
            registration.Options.Add(option);
        }

        foreach (var value in ValueBuilder.BuildValues(type))
        {
            registration.Values.Add(value);
        }

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command options type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<ICommandExecutionContext, T, Task> action)
        where T : class
        => AddCommand(registration, name, action, null);

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command options type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<ICommandExecutionContext, T, Task> action,
        Action<ICommandConfiguration>? configure)
        where T : class
    {
        var command = new AsyncDelegateCommand<T>(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command options type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<ICommandExecutionContext, T> action)
        where T : class
        => AddCommand(registration, name, action, null);

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command options type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<ICommandExecutionContext, T> action,
        Action<ICommandConfiguration>? configure)
        where T : class
    {
        var command = new DelegateCommand<T>(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<ICommandExecutionContext, Task> action)
        => AddCommand(registration, name, action, null);

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<ICommandExecutionContext, Task> action,
        Action<ICommandConfiguration>? configure)
    {
        var command = new AsyncDelegateCommand(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<ICommandExecutionContext> action)
        => AddCommand(registration, name, action, null);

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<ICommandExecutionContext> action,
        Action<ICommandConfiguration>? configure)
    {
        var command = new DelegateCommand(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command factory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="registration"></param>
    /// <param name="name"></param>
    /// <param name="factory"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IArgumentRegistrationProperties AddCommandFactory<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<T> factory,
        Action<ICommandConfiguration>? configure = null)
        where T : ICommand
    {
        var command = new DelegateCommandFactory<T>(name, factory);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }
}
