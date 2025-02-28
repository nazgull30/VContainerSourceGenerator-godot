namespace VContainerSourceGenerator.Templates;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class InjectPropertiesTemplate
{
    public static string CreateProperties(INamedTypeSymbol mainType, List<IPropertySymbol> properties)
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

    private static StringBuilder CreateStatementsForOneProperty(INamedTypeSymbol mainType, IPropertySymbol propertyInfo)
    {
        var variable = propertyInfo.Name.FirstCharToLower();
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({propertyInfo.Type.GetTypeName()}), \"{propertyInfo.Name}\", parameters, typeof({mainType.Name}));";
        var statements = new StringBuilder();

        statements.AppendLine(resolveStr);

        if (propertyInfo.SetMethod != null)
        {
            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{propertyInfo.Name} = ({propertyInfo.Type.GetTypeName()}){variable};");
        }
        else
        {
            statements.AppendLine($"InjectorUtils.InjectNotPublicProperty(instance, \"{propertyInfo.Name}\" , {variable});");
        }

        return statements;
    }
}
