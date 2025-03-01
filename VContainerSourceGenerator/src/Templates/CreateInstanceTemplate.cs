namespace VContainerSourceGenerator.Templates;

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class CreateInstanceTemplate
{
    public static string CreateInstance(INamedTypeSymbol mainType, Action<string> addUsing)
    {
        AddUsings(mainType, addUsing);
        var constructor = mainType.GetInjectableConstructor();
        var ctorParams = constructor.Parameters;

        var statements = new StringBuilder();
        var ctorParamsSb = new StringBuilder();

        foreach (var parameter in ctorParams)
        {
            var st = $"var {parameter.Name.FirstCharToLower()} = Get{parameter.Name.FirstCharToUpper()}(objResolver, parameters);";
            statements.AppendLine(st);
            ctorParamsSb.Append($"{parameter.Name.FirstCharToLower()}").Append(',');
        }

        if (ctorParamsSb.Length > 0)
        {
            ctorParamsSb.Remove(ctorParamsSb.Length - 1, 1);
        }

        var mainTypeName = mainType.IsGenericType ? mainType.GetBaseTypeNameOfGeneric() : mainType.GetTypeName().Name;
        var createInstanceStr = $"var instance = new {mainType.GetTypeName()}({ctorParamsSb});";
        statements.AppendLine(createInstanceStr);
        statements.AppendLine("Inject(instance, objResolver, parameters);");
        statements.AppendLine("return instance;");

        var code = $$"""
                    public object CreateInstance(IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        try
                        {
                            {{statements}}
                        }
                        catch (VContainerException ex)
                        {
                            throw new VContainerException(ex.InvalidType, $"Failed to resolve {{mainType.BaseType.Name}} : {ex.Message}");
                        }
                    }
""";

        return code;
    }

    private static void AddUsings(INamedTypeSymbol mainType, Action<string> addUsing)
    {
        addUsing(mainType.ContainingNamespace.ToDisplayString());
        var typeName = mainType.GetTypeName();
        foreach (var geneticType in typeName.GenericTypes)
        {
            addUsing(geneticType.ContainingNamespace.ToDisplayString());
        }
        var ctor = mainType.GetInjectableConstructor();
        var ctorParams = ctor.Parameters;
        foreach (var parameter in ctorParams)
        {
            addUsing(parameter.ContainingNamespace.ToDisplayString());
        }
    }
}
