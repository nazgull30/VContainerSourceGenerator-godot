namespace VContainerSourceGenerator;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VContainerSourceGenerator.Templates;
using VContainerSourceGenerator.Utils;

[Generator]
public class InjectorGenerator : IIncrementalGenerator
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
        foreach (var (ctx, semanticModel) in classes)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(ctx) as INamedTypeSymbol ?? throw new ArgumentException("classSymbol is null");

            var code = StructTemplate.Create(classSymbol);

            var formattedCode = code.FormatCode();

            context.AddSource($"VContainerSourceGenerator/Injectors/{classSymbol.Name}.g.cs", formattedCode);
        }
    }
}
