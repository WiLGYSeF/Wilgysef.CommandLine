using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;
using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Parser instance factory.
/// </summary>
/// <typeparam name="TInstance">Instance type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ParserInstanceFactory{TInstance}"/> class.
/// </remarks>
/// <param name="parser">Argument parser.</param>
internal class ParserInstanceFactory<TInstance>(ArgumentParser parser)
    where TInstance : class
{
    private static readonly IArgumentDeserializerStrategy[] DefaultDeserializers = PrimitiveDeserializerStrategies
        .GetPrimitiveDeserializerStrategies()
        .Append(new EnumDeserializerStrategy())
        .Concat(DateTimeDeserializerStrategies.GetDateTimeDeserializerStrategies())
        .ToArray();

    /// <summary>
    /// Value deserializers.
    /// </summary>
    public IEnumerable<IArgumentDeserializerStrategy> Deserializers { get; set; } = new List<IArgumentDeserializerStrategy>();

    /// <summary>
    /// Value list deserializers.
    /// </summary>
    public IEnumerable<IArgumentValueListDeserializerStrategy> ListDeserializers { get; set; } = new List<IArgumentValueListDeserializerStrategy>();

    /// <summary>
    /// Value aggregators.
    /// </summary>
    public IEnumerable<IArgumentValueAggregator> ValueAggregators { get; set; } = new List<IArgumentValueAggregator>();

    /// <summary>
    /// Instance value handler.
    /// </summary>
    public IInstanceValueHandler InstanceValueHandler { get; set; } = new InstancePropertyHandler<TInstance>();

    /// <summary>
    /// Propagate deserialization exceptions.
    /// </summary>
    public bool PropagateDeserializationExceptions { get; set; }

    /// <summary>
    /// Throw if a property to be set is missing.
    /// </summary>
    public bool ThrowOnMissingProperty { get; set; }

    /// <summary>
    /// Throw if more values are given than expected.
    /// </summary>
    public bool ThrowOnTooManyValues { get; set; }

    /// <summary>
    /// Parse argument tokens to instance.
    /// </summary>
    /// <param name="argTokens">Argument tokens.</param>
    /// <param name="factory">Instance factory.</param>
    /// <returns>Instance.</returns>
    public TInstance Parse(IEnumerable<ArgumentToken> argTokens, Func<TInstance>? factory)
    {
        var instance = (factory?.Invoke() ?? Activator.CreateInstance<TInstance>())
            ?? throw new Exception($"Argument parser instance is unexpectedly null");

        var context = new DeserializationContext(
            parser,
            Deserializers.Concat(DefaultDeserializers),
            ListDeserializers,
            ValueAggregators,
            InstanceValueHandler,
            instance,
            PropagateDeserializationExceptions);

        var valueMax = parser.Values.Count > 0
            ? parser.Values.Select(v => v.EndIndex ?? int.MaxValue).Max()
            : -1;
        var valueIdx = 0;

        foreach (var result in argTokens)
        {
            if (result.IsValue)
            {
                var value = GetValue(valueIdx);
                if (value != null)
                {
                    if (!InstanceValueHandler.HasValueName(value.Name))
                    {
                        if (ThrowOnMissingProperty)
                        {
                            throw new InstanceMissingPropertyException(result.Argument, result.ArgumentPosition, typeof(TInstance).Name, value.Name);
                        }
                    }
                    else
                    {
                        context.Deserialize(result, value.Name, false);
                    }
                }
                else if (ThrowOnTooManyValues && valueIdx > valueMax)
                {
                    throw new TooManyValuesException(result.Argument, result.ArgumentPosition, valueMax + 1, result.Argument);
                }

                valueIdx++;
                continue;
            }

            if (result.IsOption)
            {
                if (!InstanceValueHandler.HasValueName(result.Option!.Name))
                {
                    if (ThrowOnMissingProperty)
                    {
                        throw new InstanceMissingPropertyException(result.Argument, result.ArgumentPosition, typeof(TInstance).Name, result.Option.Name);
                    }
                }
                else
                {
                    context.Deserialize(
                        result,
                        result.Option.Name,
                        result.Option.KeepFirstValue ?? parser.KeepFirstValue);
                }
            }
        }

        return instance;

        Value? GetValue(int index)
        {
            foreach (var value in parser.Values)
            {
                if (index >= value.StartIndex
                    && (!value.EndIndex.HasValue || index <= value.EndIndex.Value))
                {
                    return value;
                }
            }

            return null;
        }
    }

    private class DeserializationContext(
        ArgumentParser parser,
        IEnumerable<IArgumentDeserializerStrategy> deserializers,
        IEnumerable<IArgumentValueListDeserializerStrategy> listDeserializers,
        IEnumerable<IArgumentValueAggregator> valueAggregators,
        IInstanceValueHandler instanceValueHandler,
        TInstance instance,
        bool propagateDeserializationExceptions)
    {
        private readonly HashSet<string> _valueNamesSet = [];

        public ArgumentToken ArgumentToken { get; private set; } = null!;

        public string ValueName { get; private set; } = null!;

        public bool KeepFirstValue { get; private set; }

        public void Deserialize(ArgumentToken argToken, string valueName, bool keepFirstValue)
        {
            ArgumentToken = argToken;
            ValueName = valueName;
            KeepFirstValue = keepFirstValue;

            if (KeepFirstValue && !_valueNamesSet.Add(valueName))
            {
                return;
            }

            var instanceValueType = instanceValueHandler.GetValueType(instance, ValueName);
            var option = ArgumentToken.Option;

            if (ArgumentToken.Values != null)
            {
                if (!Deserialize(instanceValueType, ArgumentToken.Values, out var deserializedResult))
                {
                    // if it fails to deserialize, just keep values as strings and let it fail later
                    deserializedResult = ArgumentToken.Values;
                }

                SetValue(deserializedResult);
            }
            else if (option?.Switch ?? false)
            {
                if (option.HasLongNames
                    && option.MatchesLongName(ArgumentToken.ArgumentMatch, out bool switchNegated, parser.LongNameCaseInsensitiveDefault))
                {
                    SetValue(!switchNegated);
                }
                else
                {
                    option.MatchesShortName(ArgumentToken.ArgumentMatch[0], out bool shortSwitchNegated);
                    SetValue(!shortSwitchNegated);
                }
            }
            else if (option?.Counter ?? false)
            {
                var propValue = (int?)instanceValueHandler.GetValue(instance, ValueName) ?? 0;
                SetValue(propValue + 1);
            }
            else if (instanceValueType == typeof(bool) || instanceValueType == typeof(bool?))
            {
                // if option is none of the above but is a bool, assume it is a switch
                SetValue(true);
            }
        }

        private void SetValue(object? value)
        {
            foreach (var valueAggregate in valueAggregators)
            {
                if (valueAggregate.MatchesInstanceType(typeof(TInstance))
                    && valueAggregate.SetValue(new ArgumentValueAggregatorContext(
                        instance,
                        ArgumentToken,
                        instanceValueHandler,
                        ValueName,
                        value,
                        SetValue)))
                {
                    return;
                }
            }

            SetValue(ValueName, value);
        }

        private void SetValue(string valueName, object? value)
        {
            var propValue = instanceValueHandler.GetValue(instance, valueName);
            var propValueType = propValue?.GetType()
                ?? instanceValueHandler.GetValueType(instance, valueName);

            CreateEmptyCollectionIfNull();

            if (AddRange(ref propValue, value))
            {
                if (propValueType.IsArray)
                {
                    // arrays are replaced
                    instanceValueHandler.SetValue(instance, valueName, propValue);
                }
            }
            else if (value?.GetType().IsEnumerable(out _) ?? false)
            {
                var valueItem = Enumerate(
                    value,
                    propValueType == typeof(object)
                        ? null
                        : propValueType)
                    .Last();
                instanceValueHandler.SetValue(instance, valueName, valueItem);
            }
            else
            {
                ThrowIfCannotCast(propValueType, value);
                instanceValueHandler.SetValue(instance, valueName, value);
            }

            bool CreateEmptyCollectionIfNull()
            {
                if (propValue == null)
                {
                    if (CreateGenericCollection(propValueType, [], out var collection))
                    {
                        propValue = collection;
                        instanceValueHandler.SetValue(instance, valueName, propValue);
                        return true;
                    }
                    else if (propValueType.IsArray)
                    {
                        propValue = Array.CreateInstance(propValueType.GetElementType()!, 0);
                        instanceValueHandler.SetValue(instance, valueName, propValue);
                        return true;
                    }
                    else if (CreateNongenericCollection(propValueType, [], out var nongenericCollection))
                    {
                        propValue = nongenericCollection;
                        instanceValueHandler.SetValue(instance, valueName, propValue);
                        return true;
                    }
                }

                return false;
            }
        }

        private bool Deserialize(Type type, IReadOnlyList<string> values, out object? result)
        {
            if (DeserializeTypeConverter(values, out result))
            {
                return true;
            }

            foreach (var deserializer in listDeserializers)
            {
                if (deserializer.Deserialize(
                    new ArgumentValueListDeserializerStrategyContext(
                        ArgumentToken,
                        type,
                        values,
                        ValueName,
                        KeepFirstValue,
                        deserializers),
                    out result))
                {
                    return true;
                }
            }

            var deserializedValues = DeserializeValues(
                type.IsEnumerable(out var elementType)
                    ? elementType
                    : type,
                values);
            if (deserializedValues != null)
            {
                result = deserializedValues;
                return true;
            }

            if (DeserializeStringConstructor(type, values, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        private bool DeserializeTypeConverter(IReadOnlyList<string> values, out object? result)
        {
            if (instance.GetType().GetProperty(ValueName)?.GetCustomAttributes<TypeConverterAttribute>() is not IEnumerable<TypeConverterAttribute> typeConverterAttributes)
            {
                result = null;
                return false;
            }

            foreach (var attr in typeConverterAttributes)
            {
                if (Type.GetType(attr.ConverterTypeName) is not Type converterType)
                {
                    continue;
                }

                var converter = TypeDescriptor.GetConverter(converterType);

                if (converter.GetType() == typeof(TypeConverter))
                {
                    // converter type is not registered with TypeDescriptor, create it manually
                    converter = (TypeConverter)CreateInstance(converterType, []);
                }

                if (converter.CanConvertFrom(typeof(IReadOnlyList<string>)))
                {
                    result = converter.ConvertFrom(values);
                    return true;
                }
                else if (converter.CanConvertFrom(typeof(string)))
                {
                    result = converter.ConvertFromString(KeepFirstValue
                        ? values[0]
                        : values[^1]);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private bool DeserializeStringConstructor(
            Type type,
            IReadOnlyList<string> values,
            [NotNullWhen(true)] out object? result)
        {
            var constructors = type.GetConstructors();
            var constructorStringParameters = constructors
                .Select(c => c.GetParameters())
                .Where(p => p.All(q => !q.IsIn && !q.IsOut && !q.ParameterType.IsByRef && q.ParameterType == typeof(string))
                    && p.Count(q => !q.IsOptional) <= values.Count
                    && values.Count <= p.Length)
                .OrderByDescending(p => p.Length)
                .FirstOrDefault();

            if (constructorStringParameters != null)
            {
                // deserialize values in case string deserialization was overridden
                var deserializedStringValues = DeserializeValues(values);
                if (deserializedStringValues != null)
                {
                    result = CreateInstance(type, deserializedStringValues);
                    return true;
                }
            }

            var constructorStringListParameters = constructors
                .Where(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType.IsCastableTo(typeof(IEnumerable<string>));
                })
                .FirstOrDefault();

            if (constructorStringListParameters != null)
            {
                // deserialize values in case string deserialization was overridden
                var deserializedStringValues = DeserializeValues(values);
                if (deserializedStringValues != null)
                {
                    result = CreateInstance(type, [deserializedStringValues]);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private List<object?>? DeserializeValues(Type type, IEnumerable<string> values)
        {
            foreach (var deserializer in deserializers)
            {
                try
                {
                    if (deserializer.MatchesType(type))
                    {
                        return values.Select(v => deserializer.Deserialize(type, v))
                            .ToList();
                    }
                }
                catch
                {
                    if (propagateDeserializationExceptions)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private string?[]? DeserializeValues(IEnumerable<string> values)
        {
            foreach (var deserializer in deserializers)
            {
                try
                {
                    if (deserializer.MatchesType(typeof(string)))
                    {
                        return values.Select(v => deserializer.Deserialize(typeof(string), v)?.ToString())
                            .ToArray();
                    }
                }
                catch
                {
                    if (propagateDeserializationExceptions)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private IEnumerable<object?> Enumerate(object? obj, Type? expectedType)
        {
            if (expectedType == null)
            {
                if (obj is IEnumerable enumerable && obj is not string)
                {
                    foreach (var value in enumerable)
                    {
                        yield return value;
                    }
                }
                else
                {
                    yield return obj;
                }

                yield break;
            }

            if (obj == null)
            {
                ThrowIfCannotCast(expectedType, null);
                yield return obj;
                yield break;
            }

            if (obj.GetType().IsCastableTo(expectedType))
            {
                yield return obj;
                yield break;
            }

            ThrowIfCannotCast(typeof(IEnumerable), obj);

            var enumerator = ((IEnumerable)obj).GetEnumerator();
            while (enumerator.MoveNext())
            {
                ThrowIfCannotCast(expectedType, enumerator.Current);
                yield return enumerator.Current;
            }
        }

        private bool CreateGenericCollection(
            Type type,
            IEnumerable<object?> values,
            [NotNullWhen(true)] out object? result)
        {
            if (type.GenericTypeArguments.Length != 1)
            {
                result = null;
                return false;
            }

            var genericType = type.GenericTypeArguments[0];
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(IEnumerable<>)
                || genericTypeDefinition == typeof(IReadOnlyCollection<>)
                || genericTypeDefinition == typeof(IReadOnlyList<>)
                || genericTypeDefinition == typeof(ICollection<>)
                || genericTypeDefinition == typeof(IList<>)
                || genericTypeDefinition == typeof(List<>))
            {
                return CreateCollection(typeof(List<>), genericType, nameof(List<object>.Add), out result);
            }
            else if (genericTypeDefinition == typeof(Queue<>))
            {
                return CreateCollection(typeof(Queue<>), genericType, nameof(Queue<object>.Enqueue), out result);
            }
            else if (genericTypeDefinition == typeof(Stack<>))
            {
                return CreateCollection(typeof(Stack<>), genericType, nameof(Stack<object>.Push), out result);
            }
            else if (genericTypeDefinition == typeof(LinkedList<>))
            {
                return CreateCollection(typeof(LinkedList<>), genericType, nameof(LinkedList<object>.AddLast), out result);
            }
            else if (genericTypeDefinition == typeof(IReadOnlySet<>)
                || genericTypeDefinition == typeof(ISet<>)
                || genericTypeDefinition == typeof(HashSet<>))
            {
                return CreateCollection(typeof(HashSet<>), genericType, nameof(HashSet<object>.Add), out result);
            }
            else if (genericTypeDefinition == typeof(SortedSet<>))
            {
                return CreateCollection(typeof(SortedSet<>), genericType, nameof(SortedSet<object>.Add), out result);
            }

            result = null;
            return false;

            bool CreateCollection(Type collectionType, Type genericType, string methodName, out object result)
            {
                var collType = collectionType.MakeGenericType(genericType);
                var addMethod = collType.GetMethod(methodName, [genericType])!;

                var collection = CreateInstance(collType);
                foreach (var value in values)
                {
                    ThrowIfCannotCast(genericType, value);
                    addMethod.Invoke(collection, [value]);
                }

                result = collection;
                return true;
            }
        }

        private static bool CreateNongenericCollection(
            Type type,
            IEnumerable<string> values,
            [NotNullWhen(true)] out object? result)
        {
            if (type == typeof(ICollection)
                || type == typeof(IEnumerable)
                || type == typeof(IList))
            {
                var list = new ArrayList();
                foreach (var value in values)
                {
                    list.Add(value);
                }

                result = list;
                return true;
            }

            result = null;
            return false;
        }

        private bool AddRange(ref object? collection, object? values)
        {
            if (collection == null)
            {
                return false;
            }

            var type = collection.GetType();
            if (type.IsArray)
            {
                var elementType = type.GetElementType()!;
                var valuesCount = Enumerate(values, elementType).Count();

                var collectionAsArray = (Array)collection;
                var array = Array.CreateInstance(elementType, collectionAsArray.Length + valuesCount);
                var idx = 0;

                for (var i = 0; i < collectionAsArray.Length; i++)
                {
                    array.SetValue(collectionAsArray.GetValue(i), idx++);
                }

                foreach (var value in Enumerate(values, elementType))
                {
                    array.SetValue(value, idx++);
                }

                collection = array;
                return true;
            }
            else if (type.InheritsGeneric(typeof(ICollection<>), out var collectionType))
            {
                var add = typeof(ICollection<>).MakeGenericType(collectionType)
                    .GetMethod(nameof(ICollection<object>.Add))!;

                foreach (var value in Enumerate(values, collectionType))
                {
                    add.Invoke(collection, [value]);
                }

                return true;
            }
            else if (collection is IList list)
            {
                foreach (var value in Enumerate(values, null))
                {
                    list.Add(value);
                }

                return true;
            }
            else if (type.InheritsGeneric(typeof(Queue<>), out var queueType))
            {
                var enqueue = typeof(Queue<>).MakeGenericType(queueType)
                    .GetMethod(nameof(Queue<object>.Enqueue))!;

                foreach (var value in Enumerate(values, queueType))
                {
                    enqueue.Invoke(collection, [value]);
                }

                return true;
            }
            else if (type.InheritsGeneric(typeof(Stack<>), out var stackType))
            {
                var push = typeof(Stack<>).MakeGenericType(stackType)
                    .GetMethod(nameof(Stack<object>.Push))!;

                foreach (var value in Enumerate(values, stackType))
                {
                    push.Invoke(collection, [value]);
                }

                return true;
            }

            return false;
        }

        private object CreateInstance(Type type, params object?[]? args)
        {
            try
            {
                return Activator.CreateInstance(type, args)
                    ?? throw Error($"Creating instance of '{type}' returned null for {ValueName}");
            }
            catch (Exception ex)
            {
                throw Error($"Could not create instance of '{type}' for {ValueName}", ex);
            }
        }

        private void ThrowIfCannotCast(Type expectedType, object? obj)
            => ThrowIfCannotCast(expectedType, obj?.GetType());

        private void ThrowIfCannotCast(Type expectedType, Type? other)
        {
            if (!other.IsCastableTo(expectedType))
            {
                throw TypeMismatchError(expectedType, other);
            }
        }

        private InvalidArgumentValueDeserializationException TypeMismatchError(Type expected, Type? actual)
            => Error($"The type '{expected.Name}' was expected for {ValueName}, but received '{actual?.Name ?? "null"}'");

        private InvalidArgumentValueDeserializationException Error(string message, Exception? innerException = null)
            => new(ArgumentToken, message, innerException);
    }
}
