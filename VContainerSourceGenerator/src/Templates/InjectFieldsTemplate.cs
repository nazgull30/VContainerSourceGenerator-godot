namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VContainerSourceGenerator.Utils;

public static class InjectFieldsTemplate
{
    public static string CreateInjectFields(Type mainType, List<FieldInfo> fields)
    {
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

    private static StringBuilder CreateStatementsForOneField(Type mainType, FieldInfo fieldInfo)
    {
        var variable = fieldInfo.Name.Replace("_", "");
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({fieldInfo.FieldType.GetTypeName()}), \"{fieldInfo.Name}\", parameters, typeof({mainType.Name}));";
        var statements = new StringBuilder();

        statements.AppendLine(resolveStr);

        if (fieldInfo.IsPublic)
        {
            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{fieldInfo.Name} = ({fieldInfo.FieldType.GetTypeName()}){variable};");
        }
        else
        {
            statements.AppendLine($"InjectorUtils.InjectNotPublicField(instance, \"{fieldInfo.Name}\" , {variable});");
        }

        return statements;
    }
}
