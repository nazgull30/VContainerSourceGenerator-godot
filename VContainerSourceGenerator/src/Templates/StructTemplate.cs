namespace VContainerSourceGenerator.Templates;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class StructTemplate
{
    public static string Create(INamedTypeSymbol mainType)
    {
        var usings = new List<string>();

        var mainTypeName = mainType.IsGenericType ? mainType.GetBaseTypeNameOfGeneric() : mainType.Name;

        var fields = mainType.GetInjectableFields();
        var properties = mainType.GetInjectableProperties();
        var methods = mainType.GetInjectableMethods();

        var injectInfo = InjectTemplate.Create(fields.Count, properties.Count, methods.Count);
        var createInstanceInfo = CreateInstanceTemplate.CreateInstance(mainType, usings.Add);

        var injectFieldsInfo = "";
        if (fields.Count > 0)
        {
            injectFieldsInfo = InjectFieldsTemplate.CreateInjectFields(mainType, fields, usings.Add);
        }

        var injectPropertiesInfo = "";
        if (properties.Count > 0)
        {
            injectPropertiesInfo = InjectPropertiesTemplate.CreateProperties(mainType, properties, usings.Add);
        }

        var injectMethodsInfo = "";
        if (methods.Count > 0)
        {
            injectMethodsInfo = InjectMethodsTemplate.CreateInjectMethods(mainType, methods, usings.Add);
        }

        var constructor = mainType.GetInjectableConstructor();
        var ctorParameters = constructor.Parameters;
        var sb = new StringBuilder();
        // ctorParameters.ToList().ForEach(p => sb.AppendLine(p.Name + ": " + p.Type.Name));
        // File.WriteAllText($"{mainType.Name}.txt", sb.ToString());
        var ctorParametersSb = new StringBuilder();
        foreach (var parameter in ctorParameters)
        {
            var parameterGetter = CreateMethodByCtorParameter(mainType, parameter);
            ctorParametersSb.AppendLine(parameterGetter);
        }

        var usingsSb = new StringBuilder();
        var distinctUsings = usings.Distinct();
        foreach (var u in distinctUsings)
        {
            usingsSb.AppendLine($"using {u};");
        }

        var usingsStr = usingsSb.ToString();

        var code = $$"""
using System;
using System.Collections.Generic;
using VContainer;

{{usingsStr}}

public readonly struct EnemyFactoryInjector : IInjector
{
    {{injectInfo}}

    {{createInstanceInfo}}

    {{injectFieldsInfo}}

    {{injectPropertiesInfo}}

    {{injectMethodsInfo}}

    {{ctorParametersSb}}
}
""";

        return code;
    }

    private static string CreateMethodByCtorParameter(INamedTypeSymbol mainType, IParameterSymbol parameter)
    {
        var parameterTypeStr = parameter.Type.GetTypeName();
        var variable = parameter.Name.FirstCharToLower();
        var code = $$"""
                    private {{parameterTypeStr}} Get{{parameter.Name.FirstCharToUpper()}}(IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{variable}} = ({{parameterTypeStr}})objResolver.ResolveOrParameter(typeof ({{parameterTypeStr}}), "{{parameterTypeStr}}", parameters, typeof ({{mainType.GetTypeName()}}));
                        return {{variable}};
                    }
""";
        return code;
    }
}
