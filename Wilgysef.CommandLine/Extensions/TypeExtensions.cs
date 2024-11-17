using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Wilgysef.CommandLine.Extensions;

/// <summary>
/// <see cref="Type"/> extension methods.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Checks if <paramref name="type"/> is nullable.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns><see langword="true"/> if the type is nullable, otherwise <see langword="false"/>.</returns>
    public static bool IsNullable(this Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Checks if <paramref name="type"/> is an enum.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns><see langword="true"/> if the type is an enum, otherwise <see langword="false"/>.</returns>
    public static bool IsEnum(this Type type)
    {
        return type.IsEnum
            || (type.IsValueType && (Nullable.GetUnderlyingType(type)?.IsEnum ?? false));
    }

    /// <summary>
    /// If <paramref name="type"/> is a nullable <see cref="Enum"/>, get the underlying type.
    /// If <paramref name="type"/> is an enum, return the same type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns>Enum type.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not an enum type.</exception>
    public static Type UnwrapEnum(this Type type)
    {
        if (type.IsEnum)
        {
            return type;
        }

        if (type.IsValueType && Nullable.GetUnderlyingType(type) is Type underlying && underlying.IsEnum)
        {
            return underlying;
        }

        throw new ArgumentException("Type is not an enum", nameof(type));
    }

    /// <summary>
    /// Checks if <paramref name="from"/> is castable to <paramref name="to"/>.
    /// </summary>
    /// <param name="from">Type to cast from.</param>
    /// <param name="to">Type to cast to.</param>
    /// <returns><see langword="true"/> if <paramref name="from"/> is castable to <paramref name="to"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsCastableTo(this Type? from, Type to)
    {
        if (from == null)
        {
            return to.IsNullable();
        }

        // TODO: handle implicit casting
        return to.IsAssignableFrom(from);
    }

    /// <summary>
    /// Checks if <paramref name="type"/> inherits a generic type <paramref name="ofType"/>.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="ofType">Generic type.</param>
    /// <param name="genericArgType">The generic type parameter of <paramref name="ofType"/> if it is inherited.</param>
    /// <returns><see langword="true"/> if the type inherits the generic type <paramref name="ofType"/>, otherwise <see langword="false"/>.</returns>
    public static bool InheritsGeneric(
        this Type type,
        Type ofType,
        [NotNullWhen(true)] out Type? genericArgType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == ofType)
        {
            genericArgType = type.GenericTypeArguments[0];
            return true;
        }

        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == ofType)
            {
                genericArgType = iface.GenericTypeArguments[0];
                return true;
            }
        }

        genericArgType = null;
        return false;
    }

    /// <summary>
    /// Checks if <paramref name="type"/> is an enumerable collection.
    /// </summary>
    /// <remarks>This method does not consider <see cref="string"/> as enumerable.</remarks>
    /// <param name="type">Type.</param>
    /// <param name="elementType">Element type of enumerable.</param>
    /// <returns><see langword="true"/> if the type is enumerable, otherwise <see langword="false"/>.</returns>
    public static bool IsEnumerable(this Type type, [NotNullWhen(true)] out Type? elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        if (type == typeof(string))
        {
            // do not treat string as enumerable
            elementType = null;
            return false;
        }

        if (type.InheritsGeneric(typeof(IEnumerable<>), out elementType))
        {
            return true;
        }

        if (type.IsCastableTo(typeof(IEnumerable)))
        {
            elementType = typeof(object);
            return true;
        }

        return false;
    }
}
