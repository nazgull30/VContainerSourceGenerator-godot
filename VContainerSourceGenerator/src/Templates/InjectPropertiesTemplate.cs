namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class InjectPropertiesTemplate
{
    public static string CreateProperties(INamedTypeSymbol mainType, List<IPropertySymbol> properties, Action<string> addUsing)
    {
        AddUsings(properties, addUsing);
        var statements = new StringBuilder();
        foreach (var propertyInfo in properties)
        {
            var propertyStatements = CreateStatementsForOneProperty(mainType, propertyInfo);
            statements.Append(propertyStatements);
        }


        var code = $$"""
                    private void InjectProperties(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{mainType.GetTypeName().Name.FirstCharToLower()}} = ({{mainType.GetTypeName()}}) instance;
                        {{statements}}
                    }
""";

        return code;
    }

    private static StringBuilder CreateStatementsForOneProperty(INamedTypeSymbol mainType, IPropertySymbol propertyInfo)
    {
        var variable = propertyInfo.Name.FirstCharToLower();
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({propertyInfo.Type.GetTypeName()}), \"{propertyInfo.Name}\", parameters, typeof({mainType.Name}));";
        var statements = new StringBuilder();

        statements.AppendLine(resolveStr);

        if (propertyInfo.SetMethod != null && propertyInfo.SetMethod.DeclaredAccessibility == Accessibility.Public)
        {
            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{propertyInfo.Name} = ({propertyInfo.Type.GetTypeName()}){variable};");
        }
        else
        {
            statements.AppendLine($"InjectorUtils.InjectNotPublicProperty(instance, \"{propertyInfo.Name}\" , {variable});");
        }

        return statements;
    }

    private static void AddUsings(List<IPropertySymbol> propertySymbols, Action<string> addUsing)
    {
        foreach (var propertySymbol in propertySymbols)
        {
            addUsing(propertySymbol.ContainingNamespace.ToDisplayString());
            var typeName = propertySymbol.Type.GetTypeName();
            foreach (var geneticType in typeName.GenericTypes)
            {
                addUsing(geneticType.ContainingNamespace.ToDisplayString());
            }
        }
    }
}
