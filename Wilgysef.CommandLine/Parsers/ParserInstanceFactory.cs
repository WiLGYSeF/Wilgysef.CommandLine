using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;
using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Parser instance factory.
/// </summary>
/// <typeparam name="TInstance">Instance type.</typeparam>
internal class ParserInstanceFactory<TInstance>
    where TInstance : class
{
    private static readonly IArgumentDeserializerStrategy[] DefaultDeserializers = PrimitiveDeserializerStrategies
        .GetPrimitiveDeserializerStrategies()
        .Append(new EnumDeserializerStrategy())
        .Concat(DateTimeDeserializerStrategies.GetDateTimeDeserializerStrategies())
        .ToArray();

    private readonly ArgumentParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserInstanceFactory{TInstance}"/> class.
    /// </summary>
    /// <param name="parser">Argument parser.</param>
    public ParserInstanceFactory(ArgumentParser parser)
    {
        _parser = parser;
    }

    /// <summary>
    /// Value deserializers.
    /// </summary>
    public IEnumerable<IArgumentDeserializerStrategy> Deserializers { get; set; } = new List<IArgumentDeserializerStrategy>();

    /// <summary>
    /// Value list deserializers.
    /// </summary>
    public IEnumerable<ArgumentValueListDeserializerStrategy> ListDeserializers { get; set; } = new List<ArgumentValueListDeserializerStrategy>();

    /// <summary>
    /// Value aggregators.
    /// </summary>
    public IEnumerable<IArgumentValueAggregator> ValueAggregators { get; set; } = new List<IArgumentValueAggregator>();

    /// <summary>
    /// Instance value handler.
    /// </summary>
    public IInstanceValueHandler InstanceValueHandler { get; set; } = new InstancePropertyHandler<TInstance>();

    /// <summary>
    /// Throw if a property to be set is missing.
    /// </summary>
    public bool ThrowOnMissingProperty { get; set; }

    /// <summary>
    /// Parse argument tokens to instance.
    /// </summary>
    /// <param name="argTokens">Argument tokens.</param>
    /// <param name="factory">Instance factory.</param>
    /// <returns>Instance.</returns>
    public TInstance Parse(IEnumerable<ArgumentToken> argTokens, Func<TInstance>? factory)
    {
        var instance = (factory != null ? factory() : Activator.CreateInstance<TInstance>())
            ?? throw new Exception($"Argument parser instance is unexpectedly null");

        var context = new DeserializationContext(
            _parser,
            Deserializers.Concat(DefaultDeserializers),
            ListDeserializers,
            ValueAggregators,
            InstanceValueHandler,
            instance);

        var valueIdx = 0;

        foreach (var result in argTokens)
        {
            if (result.Option == null)
            {
                var value = GetValue(valueIdx++);
                if (value != null)
                {
                    if (!InstanceValueHandler.HasValueName(value.Name))
                    {
                        if (ThrowOnMissingProperty)
                        {
                            throw new ArgumentParseException(result.Argument, result.ArgumentPosition, $"The instance \"{typeof(TInstance).Name}\" is missing the property {value.Name}");
                        }
                    }
                    else
                    {
                        context.Deserialize(result, value.Name, false);
                    }
                }

                continue;
            }

            if (!InstanceValueHandler.HasValueName(result.Option.Name))
            {
                if (ThrowOnMissingProperty)
                {
                    throw new ArgumentParseException(result.Argument, result.ArgumentPosition, $"The instance \"{typeof(TInstance).Name}\" is missing the property {result.Option.Name}");
                }
            }
            else
            {
                context.Deserialize(
                    result,
                    result.Option.Name,
                    result.Option.KeepFirstValue ?? _parser.KeepFirstValue);
            }
        }

        return instance;

        Value? GetValue(int index)
        {
            foreach (var value in _parser.Values)
            {
                if (value.PositionRange.InRange(index))
                {
                    return value;
                }
            }

            return null;
        }
    }

    private class DeserializationContext
    {
        private readonly ArgumentParser _parser;
        private readonly HashSet<string> _valueNamesSet = [];

        public DeserializationContext(
            ArgumentParser parser,
            IEnumerable<IArgumentDeserializerStrategy> deserializers,
            IEnumerable<ArgumentValueListDeserializerStrategy> listDeserializers,
            IEnumerable<IArgumentValueAggregator> valueAggregators,
            IInstanceValueHandler instanceValueHandler,
            TInstance instance)
        {
            _parser = parser;
            Deserializers = deserializers;
            ListDeserializers = listDeserializers;
            ValueAggregators = valueAggregators;
            InstanceValueHandler = instanceValueHandler;
            Instance = instance;
        }

        public TInstance Instance { get; }

        public IEnumerable<IArgumentDeserializerStrategy> Deserializers { get; }

        public IEnumerable<ArgumentValueListDeserializerStrategy> ListDeserializers { get; }

        public IEnumerable<IArgumentValueAggregator> ValueAggregators { get; }

        public IInstanceValueHandler InstanceValueHandler { get; }

        public ArgumentToken ArgumentToken { get; private set; } = null!;

        public string ValueName { get; private set; } = null!;

        public bool KeepFirstValue { get; private set; }

        public void Deserialize(ArgumentToken argToken, string valueName, bool keepFirstValue)
        {
            ArgumentToken = argToken;
            ValueName = valueName;
            KeepFirstValue = keepFirstValue;

            var instanceValueType = InstanceValueHandler.GetValueType(Instance, ValueName);
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
                    && option.MatchesLongName(ArgumentToken.ArgumentMatch, out bool switchNegated, _parser.LongNameCaseInsensitiveDefault))
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
                var propValue = (int?)InstanceValueHandler.GetValue(Instance, ValueName) ?? 0;
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
            foreach (var valueAggregate in ValueAggregators)
            {
                if (valueAggregate.MatchesInstanceType(typeof(TInstance))
                    && valueAggregate.SetValue(new ArgumentValueAggregatorContext(
                        Instance,
                        ArgumentToken,
                        InstanceValueHandler,
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
            var propValue = InstanceValueHandler.GetValue(Instance, valueName);
            var propValueType = propValue?.GetType() ?? InstanceValueHandler.GetValueType(Instance, valueName);
            var setProp = true;

            if (propValue != null)
            {
                var valueType = value?.GetType();

                if (propValueType.IsArray)
                {
                    value = CombineArray((Array)propValue, value);
                }
                else if (propValueType.InheritsGeneric(typeof(ICollection<>), out var collectionType))
                {
                    setProp = !AddTo(propValue, value, propValueType, collectionType, AddToCollection);
                }
                else if (propValue is IList list)
                {
                    if (value is not string && value is IEnumerable enumerable)
                    {
                        foreach (var val in enumerable)
                        {
                            list.Add(val);
                        }
                    }
                    else
                    {
                        list.Add(value);
                    }

                    setProp = false;
                }
                else if (propValueType.InheritsGeneric(typeof(Queue<>), out var queueType))
                {
                    setProp = !AddTo(propValue, value, propValueType, queueType, AddToQueue);
                }
                else if (propValueType.InheritsGeneric(typeof(Stack<>), out var stackType))
                {
                    setProp = !AddTo(propValue, value, propValueType, stackType, AddToStack);
                }
            }

            if (setProp
                && (!_valueNamesSet.Contains(valueName) || !KeepFirstValue))
            {
                ThrowIfCannotCast(InstanceValueHandler.GetValueType(Instance, valueName), value);
                InstanceValueHandler.SetValue(Instance, valueName, value);
                _valueNamesSet.Add(valueName);
            }

            bool AddTo(object collection, object? value, Type collectionType, Type itemType, Action<object, object?, Type> action)
            {
                var valueType = value?.GetType();
                if (valueType == null)
                {
                    ThrowIfCannotCast(itemType, null);
                    action(propValue, value, itemType);
                    return true;
                }

                if (valueType == typeof(string) && itemType.IsCastableTo(valueType))
                {
                    action(propValue, value, itemType);
                    return true;
                }
                else if (valueType.InheritsGeneric(typeof(IEnumerable<>), out var valueEnumerableType))
                {
                    ThrowIfCannotCast(itemType, valueEnumerableType);

                    foreach (var val in Enumerate(value!))
                    {
                        action(propValue, val, valueEnumerableType);
                    }

                    return true;
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var val in enumerable)
                    {
                        ThrowIfCannotCast(itemType, val?.GetType());
                        action(propValue, val, collectionType.GenericTypeArguments.FirstOrDefault() ?? typeof(object));
                    }

                    return true;
                }

                return false;
            }
        }

        private bool Deserialize(Type type, IReadOnlyList<string> values, out object? result)
        {
            foreach (var deserializer in ListDeserializers)
            {
                if (deserializer.Deserialize(
                    new ArgumentValueListDeserializerStrategy.Context(
                        ArgumentToken,
                        type,
                        values,
                        ValueName,
                        KeepFirstValue,
                        Deserializers),
                    out result))
                {
                    return true;
                }
            }

            if (DeserializeCollection(type, values, out result))
            {
                return true;
            }

            var deserializedValues = DeserializeValues(type, values);
            if (deserializedValues != null)
            {
                result = KeepFirstValue
                    ? deserializedValues.First()
                    : deserializedValues.Last();
                return true;
            }

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

        private bool DeserializeCollection(
            Type type,
            IEnumerable<string> values,
            [NotNullWhen(true)] out object? result)
        {
            if (type.IsGenericType)
            {
                return DeserializeGenericCollection(type, values, out result);
            }

            if (type.IsArray && type.GetElementType() is Type elementType)
            {
                return DeserializeArray(elementType, values, out result);
            }

            return DeserializeNongenericCollection(type, values, out result);
        }

        private bool DeserializeGenericCollection(
            Type type,
            IEnumerable<string> values,
            [NotNullWhen(true)] out object? result)
        {
            var genericType = type.GenericTypeArguments[0];
            var deserializedValues = DeserializeValues(genericType, values);
            if (deserializedValues == null)
            {
                result = null;
                return false;
            }

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
                foreach (var value in deserializedValues)
                {
                    ThrowIfCannotCast(genericType, value);
                    addMethod.Invoke(collection, [value]);
                }

                result = collection;
                return true;
            }
        }

        private bool DeserializeArray(
            Type type,
            IEnumerable<string> values,
            [NotNullWhen(true)] out object? result)
        {
            var deserializedValues = DeserializeValues(type, values);
            if (deserializedValues == null)
            {
                result = null;
                return false;
            }

            var array = Array.CreateInstance(type, deserializedValues.Count);
            for (var i = 0; i < deserializedValues.Count; i++)
            {
                ThrowIfCannotCast(type, deserializedValues[i]);
                array.SetValue(deserializedValues[i], i);
            }

            result = array;
            return true;
        }

        private bool DeserializeNongenericCollection(
            Type type,
            IEnumerable<string> values,
            [NotNullWhen(true)] out object? result)
        {
            if (type == typeof(ICollection)
                || type == typeof(IEnumerable)
                || type == typeof(IList))
            {
                var deserializedValues = DeserializeValues(values);
                if (deserializedValues != null)
                {
                    result = new ArrayList(deserializedValues);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private List<object?>? DeserializeValues(Type type, IEnumerable<string> values)
        {
            foreach (var deserializer in Deserializers)
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
                    // continue if an exception was thrown while deserializing
                }
            }

            return null;
        }

        private string?[]? DeserializeValues(IEnumerable<string> values)
        {
            foreach (var deserializer in Deserializers)
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
                    // continue if an exception was thrown while deserializing
                }
            }

            return null;
        }

        private static IEnumerable Enumerate(object obj)
        {
            var enumerator = ((IEnumerable)obj).GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        private static void AddToCollection(object collection, object? item, Type itemType)
        {
            typeof(ICollection<>).MakeGenericType(itemType)
                .GetMethod(nameof(ICollection<object>.Add))!
                .Invoke(collection, [item]);
        }

        private static void AddToQueue(object collection, object? item, Type itemType)
        {
            typeof(Queue<>).MakeGenericType(itemType)
                .GetMethod(nameof(Queue<object>.Enqueue))!
                .Invoke(collection, [item]);
        }

        private static void AddToStack(object collection, object? item, Type itemType)
        {
            typeof(Stack<>).MakeGenericType(itemType)
                .GetMethod(nameof(Stack<object>.Push))!
                .Invoke(collection, [item]);
        }

        private Array CombineArray(Array array, object? obj)
        {
            var arrayType = array.GetType().GetElementType()!;
            var otherArray = CreateArray(arrayType, obj);

            var result = Array.CreateInstance(arrayType, array.Length + otherArray.Length);
            var idx = 0;

            for (var i = 0; i < array.Length; i++)
            {
                result.SetValue(array.GetValue(i), idx++);
            }

            for (var i = 0; i < otherArray.Length; i++)
            {
                result.SetValue(otherArray.GetValue(i), idx++);
            }

            return result;
        }

        private object?[] CreateArray(Type arrayType, object? obj)
        {
            var objType = obj?.GetType();

            if (obj is not string && obj is IEnumerable enumerable)
            {
                var list = new ArrayList();
                foreach (var val in enumerable)
                {
                    ThrowIfCannotCast(arrayType, val);
                    list.Add(val);
                }

                return list.ToArray();
            }

            ThrowIfCannotCast(arrayType, objType);
            return [obj];
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

        private InvalidArgumentValueDeserializationException TypeMismatchError(Type expected, Type? actual)
        {
            return Error($"The type '{expected.Name}' was expected for {ValueName}, but received '{actual?.Name ?? "null"}'");
        }

        private InvalidArgumentValueDeserializationException Error(string message, Exception? innerException = null)
        {
            return new InvalidArgumentValueDeserializationException(ArgumentToken, message, innerException);
        }
    }
}
