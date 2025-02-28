namespace VContainerSourceGenerator.World;

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


            var typeInfo = semanticModel.GetTypeInfo(ctx).Type;
            typeInfo.ToDisplayString
            StructTemplate.Create(type.BaseType);

            var mainy = typeSymbol.Name.Remove(0, 1);
            var code = $$"""



using {{interfaceSymbol.ContainingNamespace.ToDisplayString()}};

                         public class {{className}}(Arch.Core.World world) : WorldWrapper(world), {{interfaceSymbol.Name}};

""";

            var formattedCode = code.FormatCode();

            context.AddSource($"EcsCodeGen.Worlds/{className}Wrapper.g.cs", formattedCode);
        }


    }
}
