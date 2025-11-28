using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Flow.SDK.Plugins.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Flow.SDK.Plugins.Generators;

#nullable enable

/// <summary>
/// 
/// </summary>
/// <remarks>Extensive use of AI-generated code.</remarks>
// #pragma warning disable RS1038
// #pragma warning restore RS1038
[Generator(LanguageNames.CSharp)]
public class InternalNodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // throw new NotImplementedException();
        
        context.RegisterPostInitializationOutput(Generate);
        return;

        void Generate(IncrementalGeneratorPostInitializationContext c)
        {
            // c.AddSource("Flow.GeneratorTest.g.cs", Constants.Header);

            var methodsDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (sn, _) => IsSyntaxTargetForGeneration(sn),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)!)
                .Where(static m => m is not null);
            
            var compilation = context.CompilationProvider.Combine(methodsDeclarations.Collect());
            
            context.RegisterSourceOutput(compilation, static (spc, source) => Execute(source.Left, source.Right, spc));
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
    
    private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // 检查方法是否包含 StaticNodeAttribute
        foreach (var attributeList in methodDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
                
                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Flow.SDK.Plugins.Attributes.StaticNodeAttribute")
                    return methodDeclaration;
            }
        }
        return null;
    }

    private static void Execute(
        Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
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
            var staticNodeAttribute = methodSymbol.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "Flow.SDK.Plugins.Attributes.StaticNodeAttribute");

            if (staticNodeAttribute == null)
                continue;

            // 生成节点类代码
            var sourceCode = GenerateNodeClass(methodSymbol, staticNodeAttribute);
            context.AddSource($"{methodSymbol.Name}Node.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
        
        // throw new NotImplementedException();
    }
    
    private static string GenerateNodeClass(IMethodSymbol methodSymbol, AttributeData staticNodeAttribute)
    {
        // throw new NotImplementedException();

        var className = $"{methodSymbol.Name}Node";
        var namespaceName = methodSymbol.ContainingNamespace.ToDisplayString();
        
        // 从 StaticNodeAttribute 获取名称和描述
        var nodeName = GetAttributeArgumentValue(staticNodeAttribute, "name", methodSymbol.Name);
        var nodeDescription = GetAttributeArgumentValue(staticNodeAttribute, "description", $"Node for {methodSymbol.Name} method");

        // 生成输入参数元数据
        var inputMetadata = new StringBuilder();
        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            var inputAttribute = parameter.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "Flow.SDK.Plugins.Attributes.InputAttribute");
            
            var paramName = GetAttributeArgumentValue(inputAttribute, "name", parameter.Name);
            var paramDescription = GetAttributeArgumentValue(inputAttribute, "description", $"Parameter {parameter.Name}");
            var defaultValue = GetDefaultValueForType(parameter.Type);

            inputMetadata.AppendLine(
$"""
new ParameterMetadata(
    Index: {i}, 
    Name: "{paramName}", 
    Description: "{paramDescription}", 
    Type: typeof({parameter.Type.ToDisplayString()}), 
    IsRequired: true, 
    DefaultValue: {defaultValue}),
""");
        }

        // 生成输出参数元数据
        var outputMetadata = new StringBuilder();
        outputMetadata.AppendLine(
$"""
new ParameterMetadata(
    Index: 0, 
    Name: "Result", 
    Description: "Result of {methodSymbol.Name} operation.", 
    Type: typeof({methodSymbol.ReturnType.ToDisplayString()}), 
    IsRequired: true, 
    DefaultValue: {GetDefaultValueForType(methodSymbol.ReturnType)}),
""");

        // 生成 Execute 方法体
        var executeMethodBody = GenerateExecuteMethodBody(methodSymbol);

        // 生成稳定的 GUID（基于命名空间+类名+方法名）
        var guid = GenerateDeterministicGuid(namespaceName, methodSymbol.ContainingType.Name, methodSymbol.Name);

        return 
$$"""
// Generated at {{DateTime.Now}}
{{Constants.Header}}

namespace {{namespaceName}};

public partial class {{methodSymbol.ContainingType.Name}}
{
    public class {{className}} : INode, IExecutable
    {
        private static readonly NodeMetadata NodeMetadata = new()
        {
            Id = new Guid("{{guid}}"),
            Name = "{{nodeName}}",
            Description = "{{nodeDescription}}"
        };
        
        /// <inheritdoc />
        public NodeMetadata Metadata => NodeMetadata;
        
        /// <inheritdoc />
        public Guid RuntimeId { get; init; }
        
        /// <inheritdoc />
        public NodeStatus Status { get; set; }
        
        private static readonly ParameterMetadata[]? InputMetadata =
        [
{{inputMetadata.ToString().AlignWithIndent(12)}}
        ];
        
        /// <inheritdoc />
        public ParameterMetadata[]? InputVariableMetadata => InputMetadata;
        
        /// <inheritdoc />
        public object?[]? Inputs { get; set; }
        
        private static readonly ParameterMetadata[]? OutputMetadata =
        [
{{outputMetadata.ToString().AlignWithIndent(12)}}
        ];
        
        /// <inheritdoc />
        public ParameterMetadata[]? OutputVariableMetadata => OutputMetadata;
        
        /// <inheritdoc />
        public object?[]? Outputs { get; set; }
        
        /// <inheritdoc />
        public Result? Result { get; private set; }
        
        /// <inheritdoc />
        public void Execute()
        {
            try
            {
                {{executeMethodBody}}
            }
            catch (Exception ex)
            {
                Result = new Result(
                    IsCompleted: false, 
                    IsSuccess: false, 
                    Exception: ex, 
                    Message: "Failed to execute {{methodSymbol.Name}} operation.");
            }
        }
    }
}
""";
    }
    
    private static string GenerateExecuteMethodBody(IMethodSymbol methodSymbol)
    {
        var sb = new StringBuilder();
        
        // 构建方法调用参数
        var parameters = new List<string>();
        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
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
            sb.AppendLine("                Outputs = new object?[] { result };");
        }

        sb.Append("                Result = new Result(IsCompleted: true, IsSuccess: true, Message: \"Operation completed successfully.\");");

        return sb.ToString();
    }

    private static string GetAttributeArgumentValue(AttributeData? attribute, string argumentName, string defaultValue)
    {
        if (attribute == null)
            return defaultValue;

        foreach (var arg in attribute.NamedArguments)
        {
            if (arg.Key == argumentName && arg.Value.Value is string value)
            {
                return value;
            }
        }

        return defaultValue;
    }

    private static string GetDefaultValueForType(ITypeSymbol type)
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

    private static string GenerateDeterministicGuid(string namespaceName, string className, string methodName)
    {
        var input = $"{namespaceName}.{className}.{methodName}";
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash).ToString();
    }
}