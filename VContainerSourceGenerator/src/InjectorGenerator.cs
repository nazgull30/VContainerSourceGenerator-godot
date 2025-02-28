namespace VContainerSourceGenerator.World;

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
        var interfaceDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is TypeDeclarationSyntax,
                transform: (context, _) => (context.Node as TypeDeclarationSyntax, context.SemanticModel))
            .Where(pair => Utilities.HasAttribute("PdArchEcsCore.Attributes.Generate", pair.Item1, pair.Item2))
            .Collect();

        context.RegisterSourceOutput(interfaceDeclarations, GenerateCode);
    }

    private void GenerateCode(SourceProductionContext context,
        ImmutableArray<(TypeDeclarationSyntax, SemanticModel)> interfaces)
    {
        foreach (var (ctx, semanticModel) in interfaces)
        {

            var classSymbol = semanticModel.GetDeclaredSymbol(ctx) as INamedTypeSymbol ?? throw new ArgumentException("classSymbol is null");
            var code = StructTemplate.Create(classSymbol);

            var formattedCode = code.FormatCode();

            context.AddSource($"VContainerSourceGenerator/Injectors/{classSymbol.Name}.g.cs", formattedCode);
        }


    }
}
