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

namespace Flow.SDK.Plugins.Generators;

#nullable enable

/// <summary>
/// 
/// </summary>
/// <remarks>Extensive use of AI-generated code.</remarks>
[Generator(LanguageNames.CSharp)]
public sealed class StaticNodeGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
#if ROSLYN_DEBUG
        Debugger.Launch();
#endif
        
        var methodsDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (sn, _) => GeneratorUtils.IsSyntaxTargetForGeneration(sn),
                transform: static (ctx, _) => GeneratorUtils.GetSemanticTargetForGeneration(ctx, Constants.StaticNodeAttributeDisplayString)!)
            .Where(static m => m is not null);

        // Compile with Roslyn 4.13.0 or earlier (Microsoft.CodeAnalysis <= 4.13.0);
        // otherwise, the correct generator name will not be displayed.
        var compilation = context.CompilationProvider.Combine(methodsDeclarations.Collect());

        context.RegisterSourceOutput(compilation, static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;

        foreach (var methodSyntax in methods)
        {
            var semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodSyntax) as IMethodSymbol;

            if (methodSymbol is not { IsStatic: true })
                continue;

            // 获取 StaticNodeAttribute 信息
            var staticNodeAttribute = methodSymbol
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.StaticNodeAttributeDisplayString);

            if (staticNodeAttribute == null)
                continue;

            // 生成节点类代码
            var sourceCode = GenerateNodeClass(methodSymbol, staticNodeAttribute);
            context.AddSource($"{methodSymbol.Name}Node.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
    
    private static string GenerateNodeClass(IMethodSymbol methodSymbol, AttributeData staticNodeAttribute)
    {
        var className = $"{methodSymbol.Name}Node";
        var namespaceName = methodSymbol.ContainingNamespace.ToDisplayString();
        
        // Node metadata
        var nodeName = GeneratorUtils.GetAttributeArgumentValue(staticNodeAttribute, "name", methodSymbol.Name);
        var nodeDescription = GeneratorUtils.GetAttributeArgumentValue(staticNodeAttribute, "description", $"Node for {methodSymbol.Name} method");

        // Input
        var inputMetadata = new StringBuilder();
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            var inputAttribute = parameter.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.StaticNodeAttributeDisplayString);
            
            var paramName = GeneratorUtils.GetAttributeArgumentValue(inputAttribute, "name", parameter.Name);
            var paramDescription = GeneratorUtils.GetAttributeArgumentValue(inputAttribute, "description", $"Parameter {parameter.Name}");
            var defaultValue = GeneratorUtils.GetDefaultValueForType(parameter.Type);

            inputMetadata.AppendLine(
                Constants.InputMetadataTemplate
                    .Replace("$$PARAM_INDEX$$", i.ToString())
                    .Replace("$$PARAM_NAME$$", paramName)
                    .Replace("$$PARAM_DESCRIPTION$$", paramDescription)
                    .Replace("$$PARAM_TYPE$$", parameter.Type.ToDisplayString())
                    .Replace("$$PARAM_DEFAULT_VALUE$$", defaultValue) 
            );
        }

        // Output
        var outputMetadata = new StringBuilder();
        outputMetadata.AppendLine(
            Constants.OutputMetadataTemplate
                .Replace("$$PARAM_NAME$$", methodSymbol.Name)
                .Replace("$$PARAM_TYPE$$", methodSymbol.ReturnType.ToDisplayString())
                .Replace("$$PARAM_DEFAULT_VALUE$$", GeneratorUtils.GetDefaultValueForType(methodSymbol.ReturnType)) 
        );
        
        // Generate
        var executeMethodBody = GenerateExecuteMethodBody(methodSymbol);
        var ctorBody = GenerateCtor(className, methodSymbol);
        var guid = GeneratorUtils.GenerateDeterministicGuid(namespaceName, methodSymbol.ContainingType.Name, methodSymbol.Name);

        return Constants.StaticNodeSourceTemplate
            .Replace("$$GENERATED_TIMESTAMP$$", DateTime.Now.ToString(CultureInfo.InvariantCulture))
            .Replace("$$SOURCE_HEADER$$", Constants.SourceHeader)
            .Replace("$$NAMESPACE$$", namespaceName)
            .Replace("$$COLLECTION_NAME$$", methodSymbol.ContainingType.Name)
            .Replace("$$NODE_CLASS_NAME$$", className)
            .Replace("$$NODE_GUID$$", guid)
            .Replace("$$NODE_NAME$$", nodeName)
            .Replace("$$NODE_DESCRIPTION$$", nodeDescription)
            .Replace("$$INPUT_METADATA$$", inputMetadata.ToString().AlignWithIndent(16))
            .Replace("$$OUTPUT_METADATA$$", outputMetadata.ToString().AlignWithIndent(16))
            .Replace("$$CTOR$$", ctorBody.AlignWithIndent(12))
            .Replace("$$EXECUTE_METHOD_BODY$$", executeMethodBody)
            .Replace("$$METHOD_NAME$$", methodSymbol.Name);
    }
    
    private static string GenerateExecuteMethodBody(IMethodSymbol methodSymbol)
    {
        var sb = new StringBuilder();
        
        // 构建方法调用参数
        var parameters = new List<string>();
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            parameters.Add($"({parameter.Type.ToDisplayString()})Inputs![{i}]!");
        }

        var parametersString = string.Join(", ", parameters);
        
        if (methodSymbol.ReturnType.SpecialType == SpecialType.System_Void)
        {
            sb.AppendLine($"{methodSymbol.Name}({parametersString});");
        }
        else
        {
            sb.AppendLine($"var result = {methodSymbol.Name}({parametersString});");
            sb.AppendLine("                    Outputs[0] = result;");
        }

        sb.Append("                    Result = new Result(IsCompleted: true, IsSuccess: true, Message: \"Operation completed successfully.\");");

        return sb.ToString();
    }

    private static string GenerateCtor(string className, IMethodSymbol methodSymbol)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"public {className}()\n" +
                      $"{{");        
        
        var inputLength = methodSymbol.Parameters.Length;
        const int outputLength = 1;

        sb.AppendLine($"    Inputs = new object?[{inputLength}];");
        sb.AppendLine($"    Outputs = new object?[{outputLength}];");
        sb.AppendLine("}");

        return sb.ToString();
    }
}