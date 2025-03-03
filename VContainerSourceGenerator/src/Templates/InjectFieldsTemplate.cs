namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class InjectFieldsTemplate
{
    public static string CreateInjectFields(INamedTypeSymbol mainType, List<IFieldSymbol> fields, Action<string> addUsing)
    {
        AddUsings(fields, addUsing);
        var statements = new StringBuilder();

        foreach (var fieldInfo in fields)
        {
            var fieldStatements = CreateStatementsForOneField(mainType, fieldInfo);
            statements.Append(fieldStatements);
        }


        var code = $$"""
                    private void InjectFields(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{mainType.GetTypeName().Name.FirstCharToLower()}} = ({{mainType.GetTypeName()}}) instance;
                        {{statements}}
                    }
""";

        return code;
    }

    private static StringBuilder CreateStatementsForOneField(INamedTypeSymbol mainType, IFieldSymbol fieldInfo)
    {
        var variable = fieldInfo.Name.Replace("_", "");
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({fieldInfo.Type.GetTypeName()}), \"{fieldInfo.Name}\", parameters, typeof({mainType.Name}));";
        var statements = new StringBuilder();

        statements.AppendLine(resolveStr);

        if (fieldInfo.DeclaredAccessibility == Accessibility.Public)
        {
            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{fieldInfo.Name} = ({fieldInfo.Type.GetTypeName()}){variable};");
        }
        else
        {
            statements.AppendLine($"InjectorUtils.InjectNotPublicField(instance, \"{fieldInfo.Name}\" , {variable});");
        }

        return statements;
    }

    private static void AddUsings(List<IFieldSymbol> fields, Action<string> addUsing)
    {
        foreach (var field in fields)
        {
            addUsing(field.ContainingNamespace.ToDisplayString());
            addUsing(field.Type.ContainingNamespace.ToDisplayString());
            var typeName = field.Type.GetTypeName();
            foreach (var geneticType in typeName.GenericTypes)
            {
                addUsing(geneticType.ContainingNamespace.ToDisplayString());
            }
        }
    }
}
