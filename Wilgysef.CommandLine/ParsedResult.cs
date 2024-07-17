namespace Wilgysef.CommandLine;

public record ParsedResult(IReadOnlyList<ParsedArgument> Arguments)
{
}

public record ParsedArgument(string? Name, IReadOnlyList<string>? Values)
{
    public static ParsedArgument Unparsed(string value)
    {
        return new ParsedArgument(null, new[] { value });
    }

    public static ParsedArgument NoValue(string name)
    {
        return new ParsedArgument(name, null);
    }

    public static ParsedArgument OneValue(string name, string value)
    {
        return new ParsedArgument(name, new[] { value });
    }

    public static ParsedArgument WithValues(string name, IReadOnlyList<string> values)
    {
        return new ParsedArgument(name, values.Count > 0 ? values : null);
    }
}
