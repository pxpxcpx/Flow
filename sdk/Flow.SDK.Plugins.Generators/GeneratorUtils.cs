using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flow.SDK.Plugins.Generators;

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

    internal static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    internal static MethodDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context, string target)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        
        foreach (var attributeList in methodDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
                
                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == target)
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
        var i = new string(' ', indent);

        return string.Join("\n", 
            rawString.Split('\n').Select(line => i + line));
    }
}