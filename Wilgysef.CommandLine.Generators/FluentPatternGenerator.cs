using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Wilgysef.CommandLine.Generators;

[Generator]
internal class FluentPatternGenerator : IIncrementalGenerator
{
    public const string Attribute = $$"""
namespace Wilgysef.CommandLine.Generators;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface)]
internal class GenerateFluentPatternAttribute : System.Attribute
{
}
""";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "GenerateFluentPatternAttribute.g.cs",
            SourceText.From(Attribute, Encoding.UTF8)));

        var fluentTargets = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Wilgysef.CommandLine.Generators.GenerateFluentPatternAttribute",
                predicate: static (node, _) => true,
                transform: static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol
            );

        context.RegisterSourceOutput(fluentTargets, static (spc, symbol) =>
        {
            var code = GenerateFluentExtensions(symbol);
            spc.AddSource($"{GetSourceName(symbol)}FluentExtensions.g.cs", SourceText.From(code, Encoding.UTF8));
        });
    }

    private static string GenerateFluentExtensions(INamedTypeSymbol symbol)
    {
        var symbolNamespace = symbol.ContainingNamespace.ToDisplayString();
        var symbolName = symbol.Name;
        var symbolIdentifier = GetSymbolIdentifier(symbol);
        var symbolTypeParams = GetSymbolTypeParameters(symbol);
        var symbolIdentifierCref = $"{symbolName}{GetSymbolTypeParametersForCref(symbol)}";
        var symbolTypeParamsConstraints = GetTypeParameterConstraints(symbol);

        var sb = new StringBuilder();
        sb.AppendLine($$"""
using {{symbolNamespace}};
using System;

namespace Wilgysef.CommandLine.Extensions;

public static class {{GetSourceName(symbol)}}Extensions
{
""");

        var props = GetAllSettableProperties(symbol);

        for (var i = 0; i < props.Count; i++)
        {
            var prop = props[i];
            if (prop.SetMethod == null || prop.IsReadOnly || prop.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            var propType = prop.Type.ToDisplayString();
            var propName = prop.Name;

            sb.AppendLine($$"""
    /// <summary>
    /// Sets {{propName}} of <see cref="{{symbolIdentifierCref}}"/>.
    /// </summary>
    /// <returns>
    /// The same <paramref name="obj"/> instance.
    /// </returns>
    public static {{symbolIdentifier}} With{{propName}}{{symbolTypeParams}}(this {{symbolIdentifier}} obj, {{propType}} value)
""");

            foreach (var constraint in symbolTypeParamsConstraints)
            {
                if (constraint.Length > 0)
                {
                    sb.AppendLine($$"""
        {{constraint}}
""");
                }
            }

            sb.AppendLine($$"""
    {
        obj.{{propName}} = value;
        return obj;
    }
""");
            if (i != props.Count - 1)
            {
                sb.AppendLine($"""

""");
            }
        }

        sb.AppendLine($$"""
}
""");

        return sb.ToString();
    }

    private static List<IPropertySymbol> GetAllSettableProperties(INamedTypeSymbol type)
    {
        var properties = new List<IPropertySymbol>();
        var seen = new HashSet<string>();

        var stack = new Stack<INamedTypeSymbol>();
        stack.Push(type);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current == null || current.SpecialType == SpecialType.System_Object)
            {
                continue;
            }

            foreach (var member in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (member.SetMethod != null
                    && !member.IsReadOnly
                    && member.DeclaredAccessibility == Accessibility.Public
                    && seen.Add(member.Name))
                {
                    properties.Add(member);
                }
            }

            foreach (var iface in current.AllInterfaces)
            {
                stack.Push(iface);
            }

            if (current.BaseType != null)
            {
                stack.Push(current.BaseType);
            }
        }

        return properties;
    }

    private static string GetSourceName(INamedTypeSymbol symbol)
    {
        return symbol.TypeParameters.Length > 0
            ? $"{symbol.Name}_{symbol.TypeParameters.Length}"
            : symbol.Name;
    }

    private static string GetSymbolIdentifier(INamedTypeSymbol symbol)
    {
        return symbol.TypeParameters.Length > 0
            ? $"{symbol.Name}{GetSymbolTypeParameters(symbol)}"
            : symbol.Name;
    }

    private static string GetSymbolTypeParameters(INamedTypeSymbol symbol)
    {
        return symbol.TypeParameters.Length > 0
            ? $"<{string.Join(",", symbol.TypeParameters.Select(p => p.Name))}>"
            : "";
    }

    private static string GetSymbolTypeParametersForCref(INamedTypeSymbol symbol)
    {
        return symbol.TypeParameters.Length > 0
            ? "{" + string.Join(",", symbol.TypeParameters.Select(p => p.Name)) + "}"
            : "";
    }

    private static List<string> GetTypeParameterConstraints(INamedTypeSymbol symbol)
    {
        var constraints = new List<string>();
        var constraintParts = new List<string>();

        foreach (var typeParam in symbol.TypeParameters)
        {
            constraintParts.Clear();

            foreach (var constraint in typeParam.ConstraintTypes)
            {
                constraintParts.Add(constraint.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            if (typeParam.HasConstructorConstraint)
            {
                constraintParts.Add("new()");
            }

            if (typeParam.HasReferenceTypeConstraint)
            {
                constraintParts.Insert(0, "class");
            }

            if (typeParam.HasValueTypeConstraint)
            {
                constraintParts.Insert(0, "struct");
            }

            if (constraintParts.Count > 0)
            {
                constraints.Add($"where {typeParam.Name} : {string.Join(", ", constraintParts)}");
            }
        }

        return constraints;
    }
}
