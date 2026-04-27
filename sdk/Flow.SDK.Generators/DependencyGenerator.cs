// #define ROSLYN_DEBUG
// #define THROW
// #define FORCE_THROW_WHEN_GENERATE

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

namespace Flow.SDK.Generators;

#nullable enable

// ATTENTION!
// Compile this file with Roslyn 4.13.0 or EARLIER VERSION (Microsoft.CodeAnalysis <= 4.13.0);
// otherwise, the correct generator name will not be displayed in some situation.

/// <summary>
/// Source generator used to attend required fields & properties for dependencies.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class DependencyGenerator : IIncrementalGenerator
{
    private IncrementalGeneratorInitializationContext _context;

    private ImmutableArray<ISymbol?> _types = [];

    private List<string> _usings = [];

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ROSLYN_DEBUG
        Debugger.Launch();
#endif
        _context = context;
        var declarations =
            GeneratorUtils.GetDeclarations<MemberDeclarationSyntax>(_context,
                Constants.DependencyAttributeString,
                Constants.StaticNodeAttributeString);

        var compilation = _context.CompilationProvider.Combine(declarations.Collect());

        context.RegisterSourceOutput(compilation, (spc, source) => Execute(source.Left, source.Right, spc));

#if THROW
        throw new System.NotImplementedException();
#endif
    }

    private void Execute(Compilation compilation, ImmutableArray<MemberDeclarationSyntax> classes,
        SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
            return;

        // Collect all nodes
        foreach (var classSyntax in classes)
        {
            var semanticModel = compilation.GetSemanticModel(classSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(classSyntax) is not IMethodSymbol nodeSymbol)
                continue;

            var nodeAttribute = nodeSymbol
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.StaticNodeAttributeString);

            if (nodeAttribute is null)
                continue;

            var n = nodeSymbol.ContainingNamespace.ToDisplayString();
            if (!_usings.Contains(n))
                _usings.Add(n);
            _types = _types.Add(nodeSymbol);
        }

        foreach (var dpSyntax in classes)
        {
            var semanticModel = compilation.GetSemanticModel(dpSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(dpSyntax) is not ITypeSymbol dpSymbol)
                continue;

            var dependencyAttribute = dpSymbol
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == Constants.DependencyAttributeString);

            if (dependencyAttribute is null)
                continue;

            // Generate
            var source = GenerateDependencyClass(dpSymbol, dependencyAttribute);
            context.AddSource($"{((ClassDeclarationSyntax)dpSyntax).Identifier.ToString()}Dependency.g.cs", source);
        }
    }

    private string GenerateDependencyClass(ITypeSymbol classSymbol, AttributeData staticNodeAttribute)
    {
        var className = $"{classSymbol.Name}";
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
#if FORCE_THROW_WHEN_GENERATE
        return Constants.GeneratorError.Replace("$error", e.Message);
#endif
        try
        {
            // Base
            var requiredBase = GenerateBase(classSymbol);
            var usings = GenerateUsings(_usings);

            // Field
            var i18NHelperField = GenerateI18NHelperField();

            // Nodes
            var nodeClassesItems = GenerateNodeClasses(_types);

            // Dict
            var dictItems = GenerateNodeTypesDictItem(_types);

            // I18N init
            var i18NInitializeMethod = GenerateI18NInitializeMethod(_types);

            return Constants.DependencyClassSourceTemplate
                .Replace("$generatedTimestamp", DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .Replace("$sourceHeader", Constants.DependencySourceHeader)
                .Replace("$nodeUsings", usings)
                .Replace("$namespace", namespaceName)
                .Replace("$dependencyBase", requiredBase)
                .Replace("$dependencyClassName", className)
                .Replace("$i18nHelper", i18NHelperField.AlignWithIndent(8))
                .Replace("$nodeClasses", nodeClassesItems.AlignWithIndent(12))
                .Replace("$nodeTypes", dictItems.AlignWithIndent(12))
                .Replace("$i18nInitMethod", i18NInitializeMethod.AlignWithIndent(8));
        }
        catch (Exception e)
        {
            return Constants.GeneratorError.Replace("$error", e.Message);
        }
    }

    private static string GenerateBase(ITypeSymbol classSymbol)
    {
        if (classSymbol
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == Constants.I18NRequiredAttributeString))
        {
            return Constants.DependencyI18NRequiredBase;
        }

        return Constants.DependencyBase;
    }

    private static string GenerateUsings(List<string> usings)
    {
        var sb = new StringBuilder();
        foreach (var @using in usings)
        {
            sb.AppendLine($"using {@using};");
        }

        return sb.ToString();
    }

    private static string GenerateI18NHelperField()
    {
        return Constants.DependencyI18NField;
    }

    private static string GenerateNodeClasses(ImmutableArray<ISymbol?> types)
    {
        var sb = new StringBuilder();
        try
        {
            foreach (var node in types)
            {
                if (node is null) continue;
                sb.AppendLine($"""
                               "{node.ContainingType}.{node.Name}Node",
                               """);
            }
        }
        catch
        {
            return string.Empty;
        }

        return sb.ToString();
    }

    private static string GenerateNodeTypesDictItem(ImmutableArray<ISymbol?> types)
    {
        var sb = new StringBuilder();
        foreach (var node in types)
        {
            if (node is null) continue;
            sb.AppendLine(Constants.NodeTypesDictItemTemplate
                .Replace("$nodeMetadata", $"{node.ContainingType}.{node.Name}Node.NodeMetadata")
                .Replace("$nodeType", $"typeof({node.ContainingType}.{node.Name}Node)"));
        }

        return sb.ToString();
    }

    private static string GenerateI18NInitializeMethod(ImmutableArray<ISymbol?> types)
    {
        var sb = new StringBuilder();

        foreach (var node in types)
        {
            if (node is null) continue;
            sb.AppendLine(Constants.I18NInitializeMethodBodySourceTemplate
                .Replace("$i18nInitRequiredObj", $"{node.ContainingType}.{node.Name}Node")
                .NormalizeIndent(4));
        }

        return Constants.I18NInitializeMethodHeader
            .Replace("$i18nInitMethodBody", sb.ToString());
    }
}