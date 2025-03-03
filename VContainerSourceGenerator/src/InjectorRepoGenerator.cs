namespace VContainerSourceGenerator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VContainerSourceGenerator.Utils;

[Generator]
public class InjectorRepoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (context, _) => (context.Node as ClassDeclarationSyntax, context.SemanticModel))
            .Where(pair => Utilities.HasAttribute("GenerateInjectorAttribute", pair.Item1, pair.Item2))
            .Collect();

        context.RegisterSourceOutput(classDeclarations, GenerateCode);
    }

    private void GenerateCode(SourceProductionContext context,
        ImmutableArray<(ClassDeclarationSyntax, SemanticModel)> classes)
    {
        var symbols = new List<INamedTypeSymbol>();

        foreach (var (ctx, semanticModel) in classes)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(ctx) as INamedTypeSymbol ?? throw new ArgumentException("classSymbol is null");
            symbols.Add(classSymbol);
        }

        var usings = new HashSet<string>();
        var usingsSb = new StringBuilder();
        var addsSb = new StringBuilder();
        var format = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
        foreach (var symbol in symbols)
        {
            usings.Add(symbol.ContainingNamespace.ToDisplayString());
            addsSb.AppendLine($"Injectors.Add(typeof({symbol.ToDisplayString(format)}), new {symbol.Name}Injector());");
        }
        foreach (var u in usings)
        {
            usingsSb.AppendLine($"using {u};");
        }
        var code = $$"""

namespace VContainer.Injectors;

using System;
using System.Collections.Generic;
using VContainer;

{{usingsSb}}

public class InjectorRepo : IInjectorRepo
{
    public Dictionary<Type, IInjector> Injectors
    {
        get;
    }

    public InjectorRepo()
    {
        Injectors = new Dictionary<Type, IInjector>();
        {{addsSb}}
    }
}
""";

        var formattedCode = code.FormatCode();

        context.AddSource($"VContainerSourceGenerator/InjectorRepo.g.cs", formattedCode);
    }
}
