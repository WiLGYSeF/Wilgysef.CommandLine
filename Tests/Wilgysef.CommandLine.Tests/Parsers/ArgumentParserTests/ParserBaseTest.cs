using Wilgysef.CommandLine.Parsers;

namespace Wilgysef.CommandLine.Tests.Parsers.ArgumentParserTests;

public abstract class ParserBaseTest
{
    protected static T ParseToAnonymous<T>(
        T instance,
        ArgumentParser parser,
        IEnumerable<string> args,
        ICollection<IArgumentValueAggregator<T>>? valueAggregates = null)
        where T : class
    {
        var valueHandler = new InstanceAnonymousValueHandler<T>();
        return parser.ParseTo(args, () => instance, valueAggregates, valueHandler);
    }
}
