namespace VContainerSourceGenerator;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Templates;
using Utils;

[Generator]
public class InjectorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
        .CreateSyntaxProvider(
            predicate: (node, _) => node is ClassDeclarationSyntax,
            transform: (context, _) => (context.Node as ClassDeclarationSyntax, context.SemanticModel))
        .Where(pair => Utilities.HasAttribute("GenerateInjectorAttribute", pair.Item1, pair.Item2)
                        || Utilities.HasAttribute("ExternalTypeRetrieverAttribute", pair.Item1, pair.Item2))
        .Collect();

        context.RegisterSourceOutput(classDeclarations, GenerateCode);
    }

    private void GenerateCode(SourceProductionContext context,
        ImmutableArray<(ClassDeclarationSyntax, SemanticModel)> classes)
    {
        Logger.Context = context;
        foreach (var (ctx, semanticModel) in classes)
        {
            var retriever = semanticModel.GetDeclaredSymbol(ctx) as INamedTypeSymbol ?? throw new ArgumentException("classSymbol is null");
            var isRetriever = Utilities.HasAttribute(retriever, "ExternalTypeRetrieverAttribute");
            if (isRetriever)
            {
                var fields = retriever.GetFields();
                foreach (var field in fields)
                {
                    var classSymbol = field.Type as INamedTypeSymbol;
                    GenerateCode(context, classSymbol);
                }
            }
        }

        foreach (var (ctx, semanticModel) in classes)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(ctx) as INamedTypeSymbol ?? throw new ArgumentException("classSymbol is null");
            var shouldGenerateInjector = Utilities.HasAttribute(classSymbol, "GenerateInjectorAttribute");

            if (shouldGenerateInjector)
            {
                GenerateCode(context, classSymbol);
            }
        }
    }

    private static void GenerateCode(SourceProductionContext context, INamedTypeSymbol classSymbol)
    {
        Logger.Log("GenerateCode: " + classSymbol.Name);
        var code = StructTemplate.Create(classSymbol);
        var formattedCode = code.FormatCode();
        context.AddSource($"VContainerSourceGenerator.Injectors/{classSymbol.Name}.g.cs", formattedCode);
    }
}
