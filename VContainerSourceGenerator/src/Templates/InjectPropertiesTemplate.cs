namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VContainerSourceGenerator.Utils;

public static class InjectPropertiesTemplate
{
    public static string CreateProperties(Type mainType, List<PropertyInfo> properties)
    {
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

    private static StringBuilder CreateStatementsForOneProperty(Type mainType, PropertyInfo propertyInfo)
    {
        var variable = propertyInfo.Name.FirstCharToLower();
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({propertyInfo.PropertyType.GetTypeName()}), \"{propertyInfo.Name}\", parameters, typeof({mainType.Name}));";
        var statements = new StringBuilder();

        statements.AppendLine(resolveStr);

        var setMethod = propertyInfo.GetSetMethod();
        if (setMethod != null)
        {
            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{propertyInfo.Name} = ({propertyInfo.PropertyType.GetTypeName()}){variable};");
        }
        else
        {
            statements.AppendLine($"InjectorUtils.InjectNotPublicProperty(instance, \"{propertyInfo.Name}\" , {variable});");
        }

        return statements;
    }
}
