using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Wilgysef.CommandLine.Commands;
using Wilgysef.CommandLine.Exceptions;
using Wilgysef.CommandLine.Extensions;
using Wilgysef.CommandLine.HelpMenus;
using Wilgysef.CommandLine.Options;
using Wilgysef.CommandLine.Parsers.ArgumentDeserializerStrategies;
using Wilgysef.CommandLine.Parsers.InstanceValueHandlers;
using Wilgysef.CommandLine.Values;

namespace Wilgysef.CommandLine.Parsers;

/// <summary>
/// Argument parser.
/// </summary>
public class ArgumentParser : IArgumentRegistrationProperties, IDeserializationOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParser"/> class.
    /// </summary>
    public ArgumentParser()
    {
        ArgumentParseErrorHandler = ex =>
        {
            if (ex is UnknownCommandException unknownCommand)
            {
                var helpMenu = HelpMenuProviderFactory(this, Array.Empty<ICommandConfiguration>());
                foreach (var line in helpMenu.GetUnknownCommandMessage(unknownCommand.Argument, unknownCommand.ExpectedCommands))
                {
                    OutputWriter.WriteLine(line);
                }
            }
            else if (ex is UnknownOptionException unknownOption)
            {
                var helpMenu = HelpMenuProviderFactory(this, Array.Empty<ICommandConfiguration>());
                foreach (var line in helpMenu.GetUnknownOptionMessage(unknownOption.Argument, unknownOption.ExpectedOptions))
                {
                    OutputWriter.WriteLine(line);
                }
            }
            else
            {
                OutputWriter.WriteLine($"Error: {ex.Message}");
            }
        };
    }

    /// <inheritdoc/>
    public ICollection<Option> Options { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<OptionGroup> OptionGroups { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<Value> Values { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<ICommandConfiguration> Commands { get; set; } = [];

    /// <summary>
    /// Short name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-p</c>, <c>"-"</c> is the short name prefix.
    /// </remarks>
    public string ShortNamePrefixDefault { get; set; } = "-";

    /// <summary>
    /// Long name prefix.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--option</c>, <c>"--"</c> is the long name prefix.
    /// </remarks>
    public string LongNamePrefixDefault { get; set; } = "--";

    /// <summary>
    /// Key-value separator.
    /// </summary>
    /// <remarks>
    /// If the option is <c>--key=value</c>, <c>"="</c> is the key-value separator.
    /// </remarks>
    public ICollection<string> KeyValueSeparatorsDefault { get; set; } = ["="];

    /// <summary>
    /// Indicates if short name options should use immediate characters as a value if applicable.
    /// </summary>
    /// <remarks>
    /// If the option is <c>-P3</c>, <c>3</c> is considered the immediate value.
    /// </remarks>
    public bool ShortNameImmediateValue { get; set; } = true;

    /// <summary>
    /// Indicates if the option long names are case insensitive.
    /// </summary>
    public bool LongNameCaseInsensitiveDefault { get; set; }

    /// <summary>
    /// Indicates if the first specified value should be kept for an option if it only supports one value.
    /// </summary>
    public bool KeepFirstValue { get; set; }

    /// <summary>
    /// Argument to specify subsequent arguments are literals.
    /// </summary>
    public string ArgumentLiteralSeparator { get; set; } = "--";

    /// <summary>
    /// Throw if an argument has an unexpected key-value.
    /// </summary>
    public bool ThrowOnArgumentUnexpectedKeyValue { get; set; } = true;

    /// <summary>
    /// Throw if an unknown option is encountered.
    /// </summary>
    public bool ThrowOnUnknownOptions { get; set; } = true;

    /// <summary>
    /// Ignore unknown options.
    /// </summary>
    public bool IgnoreUnknownOptions { get; set; } = true;

    /// <summary>
    /// Throw if an unknown short option is encountered.
    /// </summary>
    public bool ThrowOnUnknownShortOptions { get; set; } = true;

    /// <summary>
    /// Argument deserializers.
    /// </summary>
    public ICollection<IArgumentDeserializerStrategy> Deserializers { get; set; } = [];

    /// <summary>
    /// Argument list deserializers.
    /// </summary>
    public ICollection<IArgumentValueListDeserializerStrategy> ListDeserializers { get; set; } = [];

    /// <summary>
    /// Propagate deserialization exceptions.
    /// </summary>
    public bool PropagateDeserializationExceptions { get; set; }

    /// <summary>
    /// Throw if a property to be set is missing.
    /// </summary>
    public bool ThrowOnMissingProperty { get; set; } = true;

    /// <summary>
    /// Throw if more values are given than expected.
    /// </summary>
    public bool ThrowOnTooManyValues { get; set; }

    /// <summary>
    /// Help option to determine if the help menu provider should be invoked.
    /// </summary>
    public Option? HelpOption { get; set; } = new Option("Help")
    {
        Description = "Displays the help menu",
        ShortNames = ['h'],
        LongNames = ["help"],
    };

    /// <summary>
    /// <see cref="HelpMenuProvider"/> factory.
    /// </summary>
    public Func<ArgumentParser, IReadOnlyList<ICommandConfiguration>, HelpMenuProvider> HelpMenuProviderFactory { get; set; }
        = (parser, commandList) => new HelpMenuProvider(parser, commandList);

    /// <summary>
    /// Output writer.
    /// </summary>
    public TextWriter OutputWriter { get; set; } = Console.Error;

    /// <summary>
    /// Invoked when an argument parse exception occurs when calling <c>Execute()</c>.
    /// </summary>
    public Action<Exception>? ArgumentParseErrorHandler { get; set; }

    /// <summary>
    /// Invoked as middleware during <see cref="Execute(IEnumerable{string})"/>, etc. methods after arguments have been tokenized and before option instnaces have been configured.
    /// </summary>
    public Func<TokenizedArguments, (TokenizedArguments TokenizedArguments, int? ExitCode)>? ExecutePostTokenizeStage { get; set; }

    /// <summary>
    /// Invoked as middleware during <see cref="Execute(IEnumerable{string})"/>, etc. methods after option instances have been configured and before <see cref="ICommand.Execute(ICommandExecutionContext)"/> is called.
    /// </summary>
    public Func<IReadOnlyList<object?>, (IReadOnlyList<object?> Instances, int? ExitCode)>? ExecutePostOptionInitializationStage { get; set; }

    /// <summary>
    /// Tokenizes arguments.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <returns>Tokenized arguments.</returns>
    public TokenizedArguments Tokenize(IEnumerable<string> args)
    {
        return new Tokenizer(this).Tokenize(args);
    }

    /// <summary>
    /// Parses arguments into an <typeparamref name="TInstance"/> option instance.
    /// </summary>
    /// <typeparam name="TInstance">Option nstance type.</typeparam>
    /// <param name="args">Arguments.</param>
    /// <param name="factory">Option instance factory.</param>
    /// <param name="valueAggregators">Option instance value aggregators.</param>
    /// <param name="instanceValueHandler">Option instance value handler.</param>
    /// <returns>Option instance.</returns>
    /// <exception cref="ArgumentParseException">Thrown if multiple argument groups were parsed instead of just one.</exception>
    public TInstance ParseTo<TInstance>(
        IEnumerable<string> args,
        Func<TInstance>? factory = null,
        ICollection<IArgumentValueAggregator<TInstance>>? valueAggregators = null,
        IInstanceValueHandler<TInstance>? instanceValueHandler = null)
        where TInstance : class
    {
        var result = Tokenize(args);

        // take the first argument group with arguments
        var startIdx = result.ArgumentGroups.Count - 1;
        for (var i = 0; i < result.ArgumentGroups.Count; i++)
        {
            if (result.ArgumentGroups[i].Arguments.Count > 0)
            {
                startIdx = i;
                break;
            }
        }

        if (startIdx != result.ArgumentGroups.Count - 1)
        {
            var command = result.ArgumentGroups[startIdx + 1].CommandMatch
                ?? throw new Exception("Command was unexpectedly null");
            throw new ArgumentParseException(command.Argument, command.ArgumentPosition, $"{nameof(ParseTo)}() was called, but multiple argument groups were parsed");
        }

        return ParseTo(
            result.ArgumentGroups[startIdx].Arguments,
            factory,
            valueAggregators,
            instanceValueHandler);
    }

    /// <summary>
    /// Parses arguments into an <typeparamref name="TInstance"/> option instance.
    /// </summary>
    /// <typeparam name="TInstance">Option instance type.</typeparam>
    /// <param name="argTokens">Argument tokens.</param>
    /// <param name="factory">Option instance factory.</param>
    /// <param name="valueAggregators">Option instance value aggregators.</param>
    /// <param name="instanceValueHandler">Option instance value handler.</param>
    /// <returns>Option instance.</returns>
    public TInstance ParseTo<TInstance>(
        IEnumerable<ArgumentToken> argTokens,
        Func<TInstance>? factory = null,
        ICollection<IArgumentValueAggregator<TInstance>>? valueAggregators = null,
        IInstanceValueHandler<TInstance>? instanceValueHandler = null)
        where TInstance : class
    {
        return CreateRootParserInstanceFactory<TInstance>(valueAggregators, instanceValueHandler)
            .Parse(argTokens, factory);
    }

    /// <summary>
    /// Parses arguments into option instances.
    /// </summary>
    /// <typeparam name="TRootInstance">Option instance type of the first argument group (root instance).</typeparam>
    /// <param name="tokenizedArgs">Tokenized arguments.</param>
    /// <param name="rootFactory">Root option instance factory.</param>
    /// <param name="rootValueAggregators">Root option instance value aggregators.</param>
    /// <param name="rootInstanceValueHandler">Root option instance value handler.</param>
    /// <returns>Option instances.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="tokenizedArgs"/> is invalid.</exception>
    public IReadOnlyList<object?> ParseTo<TRootInstance>(
        TokenizedArguments tokenizedArgs,
        Func<TRootInstance>? rootFactory = null,
        ICollection<IArgumentValueAggregator<TRootInstance>>? rootValueAggregators = null,
        IInstanceValueHandler<TRootInstance>? rootInstanceValueHandler = null)
        where TRootInstance : class
    {
        var instances = new List<object?>();

        if (tokenizedArgs.ArgumentGroups.Count == 0)
        {
            return instances;
        }

        if (tokenizedArgs.ArgumentGroups[0].CommandMatch != null)
        {
            throw new ArgumentException("Command unexpectedly matched in first argument group", nameof(tokenizedArgs));
        }

        instances.Add(ParseTo(tokenizedArgs.ArgumentGroups[0].Arguments, rootFactory, rootValueAggregators, rootInstanceValueHandler));

        for (var i = 1; i < tokenizedArgs.ArgumentGroups.Count; i++)
        {
            var group = tokenizedArgs.ArgumentGroups[i];
            var command = group.Command
                ?? throw new ArgumentException("Command unexpectedly null in argument group", nameof(tokenizedArgs));

            var commandType = command.GetType();
            if (commandType.InheritsGeneric(typeof(IAsyncCommand<>), out var asyncOptionsType))
            {
                instances.Add(Parse(commandType, command, asyncOptionsType, group));
            }
            else if (commandType.InheritsGeneric(typeof(ICommand<>), out var optionsType))
            {
                instances.Add(Parse(commandType, command, optionsType, group));
            }
            else
            {
                instances.Add(null);
            }
        }

        return instances;

        object Parse(Type commandType, ICommandConfiguration command, Type optionsType, ArgumentTokenGroup argGroup)
        {
            // we need to create instance factories with dynamic types
            var createParserResultFactoryMethod = typeof(ArgumentParser).GetMethod(nameof(CreateParserInstanceFactory), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(optionsType)!;
            var argParserResultFactory = createParserResultFactoryMethod.Invoke(
                this,
                [
                    Deserializers.Concat(command.Deserializers),
                    ListDeserializers.Concat(command.ListDeserializers),
                    command.ThrowOnMissingProperty ?? ThrowOnMissingProperty,
                    command.ThrowOnTooManyValues ?? ThrowOnTooManyValues,
                    command.ValueAggregators ?? Enumerable.Empty<IArgumentValueAggregator>(),
                    command.InstanceValueHandler,
                ])!;
            var parseMethod = argParserResultFactory.GetType().GetMethod(nameof(ParserInstanceFactory<object>.Parse))!;

            // get the command's options factory method
            var makeMethod = typeof(ArgumentParser).GetMethod(nameof(MakeMethod), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(optionsType);
            var optionsTypeFactory = makeMethod.Invoke(null, [command, commandType.GetMethod(nameof(ICommand<object>.OptionsFactory))!]);

            return parseMethod.Invoke(argParserResultFactory, [argGroup.Arguments, optionsTypeFactory])!;
        }
    }

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <typeparam name="TRootInstance">Option instance type of the first argument group (root instance).</typeparam>
    /// <param name="args">Arguments.</param>
    /// <param name="rootHandler">Root option instance handler.</param>
    /// <param name="rootFactory">Root option instance factory.</param>
    /// <param name="rootValueAggregators">Root option instance value aggregators.</param>
    /// <param name="rootInstanceValueHandler">Root option instance value handler.</param>
    /// <returns>Exit code from executed commands.</returns>
    public int Execute<TRootInstance>(
        IEnumerable<string> args,
        Action<CommandExecutionContext, TRootInstance>? rootHandler,
        Func<TRootInstance>? rootFactory = null,
        ICollection<IArgumentValueAggregator<TRootInstance>>? rootValueAggregators = null,
        IInstanceValueHandler<TRootInstance>? rootInstanceValueHandler = null)
        where TRootInstance : class
    {
        try
        {
            if (!ExecuteSetup(
                args,
                out var tokenizedArgs,
                out var instances,
                out var exitCode,
                rootFactory,
                rootValueAggregators,
                rootInstanceValueHandler))
            {
                return exitCode.Value;
            }

            return Execute(tokenizedArgs, instances, rootHandler, args);
        }
        catch (Exception ex)
        {
            if ((ex is ArgumentParseException || ex is InvalidOptionException)
                && ArgumentParseErrorHandler != null)
            {
                ArgumentParseErrorHandler(ex);
                return 1;
            }

            throw;
        }
    }

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <returns>Exit code from executed commands.</returns>
    public int Execute(IEnumerable<string> args)
        => Execute<object>(args, null);

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <typeparam name="TRootInstance">Option instance type of the first argument group (root instance).</typeparam>
    /// <param name="tokenizedArgs">Tokenized arguments.</param>
    /// <param name="instances">Argument group instances.</param>
    /// <param name="rootHandler">Root option instance handler.</param>
    /// <param name="args">Arguments.</param>
    /// <returns>Exit code from executed commands.</returns>
    public int Execute<TRootInstance>(
        TokenizedArguments tokenizedArgs,
        IReadOnlyList<object?> instances,
        Action<CommandExecutionContext, TRootInstance>? rootHandler,
        IEnumerable<string>? args = null)
        where TRootInstance : class
    {
        using var tokenSource = new CancellationTokenSource();
        var context = new CommandExecutionContext(
            tokenizedArgs,
            args,
            tokenizedArgs.ArgumentGroups[0],
            tokenSource);

        rootHandler?.Invoke(context, (TRootInstance)instances[0]!);

        // not actually async here
        _ = ExecuteCommandAsync(tokenizedArgs, context, instances, async: false);
        return context.ExitCode;
    }

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <typeparam name="TRootInstance">Option nstance type of the first argument group (root instance).</typeparam>
    /// <param name="args">Arguments.</param>
    /// <param name="rootHandler">Root option instance handler.</param>
    /// <param name="rootFactory">Root option instance factory.</param>
    /// <param name="rootValueAggregators">Root option instance value aggregators.</param>
    /// <param name="rootInstanceValueHandler">Root option instance value handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code from executed commands.</returns>
    public async Task<int> ExecuteAsync<TRootInstance>(
        IEnumerable<string> args,
        Func<CommandExecutionContext, TRootInstance, Task>? rootHandler,
        Func<TRootInstance>? rootFactory = null,
        ICollection<IArgumentValueAggregator<TRootInstance>>? rootValueAggregators = null,
        IInstanceValueHandler<TRootInstance>? rootInstanceValueHandler = null,
        CancellationToken cancellationToken = default)
        where TRootInstance : class
    {
        try
        {
            if (!ExecuteSetup(
                args,
                out var tokenizedArgs,
                out var instances,
                out var exitCode,
                rootFactory,
                rootValueAggregators,
                rootInstanceValueHandler))
            {
                return exitCode.Value;
            }

            return await ExecuteAsync(tokenizedArgs, instances, rootHandler, args, cancellationToken);
        }
        catch (Exception ex)
        {
            if ((ex is ArgumentParseException || ex is InvalidOptionException)
                && ArgumentParseErrorHandler != null)
            {
                ArgumentParseErrorHandler(ex);
                return 1;
            }

            throw;
        }
    }

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code from executed commands.</returns>
    public Task<int> ExecuteAsync(
        IEnumerable<string> args,
        CancellationToken cancellationToken = default)
        => ExecuteAsync<object>(args, null, cancellationToken: cancellationToken);

    /// <summary>
    /// Parses arguments and executes commands.
    /// </summary>
    /// <typeparam name="TRootInstance">Option instance type of the first argument group (root instance).</typeparam>
    /// <param name="tokenizedArgs">Tokenized arguments.</param>
    /// <param name="instances">Argument group instances.</param>
    /// <param name="rootHandler">Root option instance handler.</param>
    /// <param name="args">Arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code from executed commands.</returns>
    public async Task<int> ExecuteAsync<TRootInstance>(
        TokenizedArguments tokenizedArgs,
        IReadOnlyList<object?> instances,
        Func<CommandExecutionContext, TRootInstance, Task>? rootHandler,
        IEnumerable<string>? args = null,
        CancellationToken cancellationToken = default)
        where TRootInstance : class
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var context = new CommandExecutionContext(
            tokenizedArgs,
            args,
            tokenizedArgs.ArgumentGroups[0],
            tokenSource);

        await (rootHandler?.Invoke(context, (TRootInstance)instances[0]!) ?? Task.CompletedTask);
        return await ExecuteCommandAsync(tokenizedArgs, context, instances, true);
    }

    private bool ExecuteSetup<TRootInstance>(
        IEnumerable<string> args,
        out TokenizedArguments tokenizedArgs,
        [NotNullWhen(true)] out IReadOnlyList<object?>? instances,
        [NotNullWhen(false)] out int? exitCode,
        Func<TRootInstance>? rootFactory = null,
        ICollection<IArgumentValueAggregator<TRootInstance>>? rootValueAggregators = null,
        IInstanceValueHandler<TRootInstance>? rootInstanceValueHandler = null)
        where TRootInstance : class
    {
        tokenizedArgs = Tokenize(args);

        if (HasHelpOption(tokenizedArgs) is List<ICommandConfiguration> commands)
        {
            var helpProvider = HelpMenuProviderFactory(this, commands);
            foreach (var line in helpProvider.GetHelpMenu())
            {
                OutputWriter.WriteLine(line);
            }

            instances = null;
            exitCode = 0;
            return false;
        }

        if (ExecutePostTokenizeStage != null)
        {
            (tokenizedArgs, exitCode) = ExecutePostTokenizeStage(tokenizedArgs);
            if (exitCode.HasValue)
            {
                instances = null;
                return false;
            }
        }

        instances = ParseTo(tokenizedArgs, rootFactory, rootValueAggregators, rootInstanceValueHandler);

        if (ExecutePostOptionInitializationStage != null)
        {
            (instances, exitCode) = ExecutePostOptionInitializationStage(instances);
            if (exitCode.HasValue)
            {
                return false;
            }
        }

        exitCode = null;
        return true;
    }

    private async Task<int> ExecuteCommandAsync(
        TokenizedArguments tokenizedArgs,
        CommandExecutionContext context,
        IReadOnlyList<object?> instances,
        bool async)
    {
        for (var i = 1; i < instances.Count && !context.CancellationTokenSource.IsCancellationRequested; i++)
        {
            var command = tokenizedArgs.ArgumentGroups[i].Command!;
            var commandType = command.GetType();

            if (command is ICommandFactory commandFactory)
            {
                command = commandFactory.Create();
                commandType = command.GetType();
            }

            context.ArgumentGroup = tokenizedArgs.ArgumentGroups[i];

            if (async)
            {
                if (commandType.InheritsGeneric(typeof(IAsyncCommand<>), out _))
                {
                    var execute = commandType.GetMethods()
                        .Where(m => m.Name == nameof(IAsyncCommand<object>.ExecuteAsync)
                            && m.GetParameters() is ParameterInfo[] parameters
                            && parameters.Length == 2
                            && parameters[0].ParameterType == typeof(ICommandExecutionContext))
                        .Single();
                    await (Task)execute.Invoke(command, [context, instances[i]])!;
                    continue;
                }
                else if (command is IAsyncCommand asyncCommand)
                {
                    await asyncCommand.ExecuteAsync(context);
                    continue;
                }

                // not an async command, invoke the synchronous version
            }

            if (commandType.InheritsGeneric(typeof(ICommand<>), out _))
            {
                var execute = commandType.GetMethods()
                    .Where(m => m.Name == nameof(ICommand<object>.Execute)
                        && m.GetParameters() is ParameterInfo[] parameters
                        && parameters.Length == 2
                        && parameters[0].ParameterType == typeof(ICommandExecutionContext))
                    .Single();
                execute.Invoke(command, [context, instances[i]]);
            }
            else if (command is ICommand command1)
            {
                command1.Execute(context);
            }
            else
            {
                throw new Exception($"{commandType} is not a {nameof(ICommand)} or {nameof(ICommandFactory)}");
            }
        }

        return context.ExitCode;
    }

    private List<ICommandConfiguration>? HasHelpOption(TokenizedArguments tokenizedArgs)
    {
        for (var i = 0; i < tokenizedArgs.ArgumentGroups.Count; i++)
        {
            var group = tokenizedArgs.ArgumentGroups[i];

            foreach (var arg in group.Arguments)
            {
                if (arg.Option == HelpOption)
                {
                    return tokenizedArgs.ArgumentGroups
                        .Skip(1).Take(i)
                        .Select(a => a.Command!)
                        .ToList();
                }
            }
        }

        return null;
    }

    private ParserInstanceFactory<T> CreateRootParserInstanceFactory<T>(
        IEnumerable<IArgumentValueAggregator>? valueAggregators,
        IInstanceValueHandler? instanceValueHandler)
        where T : class
    {
        return CreateParserInstanceFactory<T>(
            Deserializers,
            ListDeserializers,
            ThrowOnMissingProperty,
            ThrowOnTooManyValues,
            valueAggregators,
            instanceValueHandler);
    }

    private ParserInstanceFactory<T> CreateParserInstanceFactory<T>(
        IEnumerable<IArgumentDeserializerStrategy> deserializers,
        IEnumerable<IArgumentValueListDeserializerStrategy> listDeserializers,
        bool throwOnMissingProperty,
        bool throwOnTooManyValues,
        IEnumerable<IArgumentValueAggregator>? valueAggregators,
        IInstanceValueHandler? instanceValueHandler)
        where T : class
    {
        var factory = new ParserInstanceFactory<T>(this)
        {
            Deserializers = deserializers,
            ListDeserializers = listDeserializers,
            PropagateDeserializationExceptions = PropagateDeserializationExceptions,
            ThrowOnMissingProperty = throwOnMissingProperty,
            ThrowOnTooManyValues = throwOnTooManyValues,
        };

        if (valueAggregators != null)
        {
            factory.ValueAggregators = valueAggregators;
        }

        if (instanceValueHandler != null)
        {
            factory.InstanceValueHandler = instanceValueHandler;
        }

        return factory;
    }

    /// <summary>
    /// Gets the function delegate of <paramref name="methodInfo"/>.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="instance">Instance.</param>
    /// <param name="methodInfo">Method info.</param>
    /// <returns>Function delegate.</returns>
    private static Func<T> MakeMethod<T>(object instance, MethodInfo methodInfo)
    {
        return Expression.Lambda<Func<T>>(Expression.Call(Expression.Constant(instance), methodInfo))
            .Compile();
    }
}
