// #define ROSLYN_DEBUG

// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Flow.SDK.Generators;

#nullable enable

/// <summary>
/// Source generator which turn static async methods into async nodes.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class StaticAsyncNodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ROSLYN_DEBUG
        Debugger.Launch();
#endif
        var methodsDeclarations =
            GeneratorUtils.GetDeclarations<MethodDeclarationSyntax>(context, Constants.StaticAsyncNodeAttributeString);

        var compilation = context.CompilationProvider.Combine(methodsDeclarations.Collect());

        context.RegisterSourceOutput(compilation, static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods,
        SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;
        
        foreach (var methodSyntax in methods)
        {
            var semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);

            // Only for static method
            if (semanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol { IsStatic: true } methodSymbol)
                continue;

            // Get StaticNodeAttribute
            var staticNodeAttribute = methodSymbol
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.StaticAsyncNodeAttributeString);

            if (staticNodeAttribute is null)
                continue;

            // Generate
            var source = GenerateNodeClass(methodSymbol, staticNodeAttribute);
            context.AddSource($"{methodSymbol.Name}AsyncNode.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }
    
        private static string GenerateNodeClass(IMethodSymbol methodSymbol, AttributeData staticNodeAttribute)
    {
        var className = $"{methodSymbol.Name}AsyncNode";
        var namespaceName = methodSymbol.ContainingNamespace.ToDisplayString();

        // Node metadata
        var nodeName = GeneratorUtils.GetAttributeArgumentValue(staticNodeAttribute, "name", methodSymbol.Name);
        var nodeDescription = GeneratorUtils.GetAttributeArgumentValue(staticNodeAttribute, "description",
            $"Node for {methodSymbol.Name} method");

        // Input
        var inputMetadata = new StringBuilder();
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            var inputAttribute = parameter.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.StaticAsyncNodeAttributeString);

            var paramName = GeneratorUtils.GetAttributeArgumentValue(inputAttribute, "name", parameter.Name);
            var paramDescription =
                GeneratorUtils.GetAttributeArgumentValue(inputAttribute, "description", $"Parameter {parameter.Name}");
            var defaultValue = GeneratorUtils.GetDefaultValueForType(parameter.Type);

            inputMetadata.AppendLine(
                Constants.InputMetadataTemplate
                    .Replace("$paramIndex", i.ToString())
                    .Replace("$paramName", paramName)
                    .Replace("$paramDescription", paramDescription)
                    .Replace("$paramType", parameter.Type.ToDisplayString())
                    .Replace("$paramDefaultValue", defaultValue)
            );
        }

        // Output
        var outputMetadata = new StringBuilder();
        outputMetadata.AppendLine(
            Constants.OutputMetadataTemplate
                .Replace("$paramName", methodSymbol.Name)
                .Replace("$paramType", methodSymbol.ReturnType.ToDisplayString())
                .Replace("$paramDefaultValue", GeneratorUtils.GetDefaultValueForType(methodSymbol.ReturnType))
        );

        // Generate
        var executeMethodBody = GenerateExecuteMethodBody(methodSymbol);
        var ctorBody = GenerateCtor(className, methodSymbol);
        var guid = GeneratorUtils.GenerateDeterministicGuid(namespaceName, methodSymbol.ContainingType.Name,
            methodSymbol.Name);

        return Constants.StaticAsyncNodeSourceTemplate
            .Replace("$generatedTimestamp", DateTime.Now.ToString(CultureInfo.InvariantCulture))
            .Replace("$sourceHeader", Constants.StaticAsyncNodeSourceHeader)
            .Replace("$namespace", namespaceName)
            .Replace("$collectionName", methodSymbol.ContainingType.Name)
            .Replace("$nodeClassName", className)
            .Replace("$nodeGuid", guid)
            .Replace("$nodeName", nodeName)
            .Replace("$nodeDescription", nodeDescription)
            .Replace("$inputMetadata", inputMetadata.ToString().AlignWithIndent(16))
            .Replace("$outputMetadata", outputMetadata.ToString().AlignWithIndent(16))
            .Replace("$ctor", ctorBody.AlignWithIndent(12))
            .Replace("$executeMethodBody", executeMethodBody.AlignWithIndent(20))
            .Replace("$method_name", methodSymbol.Name);
    }
        
    private static string GenerateExecuteMethodBody(IMethodSymbol methodSymbol)
    {
        var sb = new StringBuilder();

        // Call Parameters
        var parameters = new List<string>();
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            parameters.Add(Constants.StaticAsyncNodeExecuteParamTemplate
                .Replace("$index", i.ToString())
                .Replace("$paramType", parameter.Type.ToDisplayString()));
        }

        var parametersString = string.Join(", ", parameters);

        if (methodSymbol.ReturnType.SpecialType == SpecialType.System_Void)
        {
            sb.AppendLine(Constants.StaticAsyncNodeExecuteCallVoidOriginTemplate
                .Replace("$originMethodName",  methodSymbol.Name)
                .Replace("$paramString", parametersString));
        }
        else
        {
            sb.AppendLine(Constants.StaticNodeExecuteCallNonVoidOriginTemplate
                .Replace("$originMethodName",  methodSymbol.Name)
                .Replace("$paramString", parametersString));
        }

        sb.Append(Constants.StaticNodeExecuteSuccessfullyResult);

        return sb.ToString();
    }

    private static string GenerateCtor(string className, IMethodSymbol methodSymbol)
    {
        var inputLength = methodSymbol.Parameters.Length;
        const int outputLength = 1;

        return Constants.StaticNodeCtorTemplate
            .Replace("$className", className)
            .Replace("$inputLength", inputLength.ToString())
            .Replace("$outputLength", outputLength.ToString());
    }
}