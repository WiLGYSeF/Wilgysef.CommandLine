using Wilgysef.CommandLine.Attributes;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Extensions;

/// <summary>
/// <see cref="IArgumentRegistrationProperties"/> extension methods.
/// </summary>
public static class ArgumentRegistrationPropertiesExtensions
{
    /// <summary>
    /// Configures <paramref name="registration"/> with options from <typeparamref name="T"/> specified by <see cref="OptionAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithOptionsFrom<T>(this IArgumentRegistrationProperties registration)
        where T : class
        => WithOptionsFrom(registration, typeof(T));

    /// <summary>
    /// Configures <paramref name="registration"/> with options from <paramref name="type"/> specified by <see cref="OptionAttribute"/>s.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="type">Object type.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithOptionsFrom(
        this IArgumentRegistrationProperties registration,
        Type type)
    {
        foreach (var option in OptionBuilder.BuildOptions(type))
        {
            registration.Options.Add(option);
        }

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with values from <typeparamref name="T"/> specified by <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithValuesFrom<T>(this IArgumentRegistrationProperties registration)
        where T : class
        => WithValuesFrom(registration, typeof(T));

    /// <summary>
    /// Configures <paramref name="registration"/> with values from <paramref name="type"/> specified by <see cref="ValueAttribute"/>s.
    /// </summary>
    /// <param name="registration">Registration.</param>
    /// <param name="type">Object type.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties WithValuesFrom(
        this IArgumentRegistrationProperties registration,
        Type type)
    {
        foreach (var value in ValueBuilder.BuildValues(type))
        {
            registration.Values.Add(value);
        }

        return registration;
    }

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
        => registration.WithOptionsFrom(type).WithValuesFrom(type);

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command settings type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<CommandExecutionContext, T, Task> action,
        Action<IAsyncCommand<T>>? configure = null)
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
    /// <typeparam name="T">Command settings type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand<T>(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<CommandExecutionContext, T> action,
        Action<ICommand<T>>? configure = null)
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
    /// <typeparam name="T">Command settings type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Func<CommandExecutionContext, Task> action,
        Action<IAsyncCommand>? configure = null)
    {
        var command = new AsyncDelegateCommand(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }

    /// <summary>
    /// Configures <paramref name="registration"/> with a new command.
    /// </summary>
    /// <typeparam name="T">Command settings type.</typeparam>
    /// <param name="registration">Registration.</param>
    /// <param name="name">Command name.</param>
    /// <param name="action">Command execution action.</param>
    /// <param name="configure">Command configuration.</param>
    /// <returns><paramref name="registration"/>.</returns>
    public static IArgumentRegistrationProperties AddCommand(
        this IArgumentRegistrationProperties registration,
        string name,
        Action<CommandExecutionContext> action,
        Action<ICommand>? configure = null)
    {
        var command = new DelegateCommand(name, action);
        configure?.Invoke(command);
        registration.Commands.Add(command);

        return registration;
    }
}
