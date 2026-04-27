using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flow.SDK.Generators;

internal static class GeneratorUtils
{
    internal static string GetDefaultValueForType(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Int32 => "0",
            SpecialType.System_Double => "0.0",
            SpecialType.System_Boolean => "false",
            SpecialType.System_String => "string.Empty",
            SpecialType.System_Object => "null",
            _ => type.Name switch
            {
                "String" => "string.Empty",
                _ => type.IsValueType ? $"default({type.ToDisplayString()})" : "null"
            }
        };
    }

    internal static IncrementalValuesProvider<TDeclaration> GetDeclarations<TDeclaration>(
        IncrementalGeneratorInitializationContext context, params string[] targetAttributeDisplayName)
        where TDeclaration : MemberDeclarationSyntax
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (sn, _) => sn is TDeclaration { AttributeLists.Count: > 0 },
                transform: (ctx, _) =>
                    GetSemanticTargetForGeneration<TDeclaration>(ctx, targetAttributeDisplayName)!)
            .Where(static c => c is not null);
    }

    internal static T GetSemanticTargetForGeneration<T>(GeneratorSyntaxContext context, params string[] target)
        where T : MemberDeclarationSyntax
    {
        var methodDeclaration = (T)context.Node;

        foreach (var attributeList in methodDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (target.Contains(fullName))
                    return methodDeclaration;
            }
        }

        return null;
    }

    internal static string GetAttributeArgumentValue(AttributeData attribute, string argumentName, string defaultValue)
    {
        if (attribute == null)
            return defaultValue;

        foreach (var arg in attribute.NamedArguments)
        {
            if (arg.Key == argumentName && arg.Value.Value is string value)
                return value;
        }

        return defaultValue;
    }

    internal static string GenerateDeterministicGuid(string namespaceName, string className, string methodName)
    {
        var input = $"{namespaceName}.{className}.{methodName}";
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash).ToString();
    }

    internal static string AlignWithIndent(this string rawString, int indent)
    {
        var intent = new string(' ', indent);

        return string
            .Join("\n", rawString
                .Split(["\r\n", "\n"], StringSplitOptions.None)
                .Select(line => intent + line));
    }
    
    public static string NormalizeIndent(this string code, int indent)
    {
        if (string.IsNullOrEmpty(code))
            return code;

        var desiredIndent = new string(' ', indent);
        var lines = code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        
        int minLeadingSpaces = int.MaxValue;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            var match = Regex.Match(line, @"^[ \t]+");
            if (match.Success)
            {
                int leadingLength = match.Length;
                if (leadingLength < minLeadingSpaces)
                    minLeadingSpaces = leadingLength;
            }
            else
            {
                minLeadingSpaces = 0;
                break;
            }
        }

        if (minLeadingSpaces == int.MaxValue)
            minLeadingSpaces = 0;
        
        var normalizedLines = lines.Select(line =>
        {
            if (string.IsNullOrWhiteSpace(line))
                return line;

            if (line.Length >= minLeadingSpaces)
                line = line.Substring(minLeadingSpaces);
            return desiredIndent + line;
        });

        return string.Join("\r\n", normalizedLines);
    }
}