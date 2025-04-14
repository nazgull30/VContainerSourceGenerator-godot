namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class StructTemplate
{
    public static string Create(INamedTypeSymbol mainType)
    {
        var usings = new HashSet<string>();

        var fields = mainType.GetInjectableFields();
        var properties = mainType.GetInjectableProperties();
        var methods = mainType.GetInjectableMethods();

        var injectInfo = InjectTemplate.Create(fields.Count, properties.Count, methods.Count);
        var createInstanceInfo = CreateInstanceTemplate.CreateInstance(mainType, (u) => usings.Add(u));

        var injectFieldsInfo = "";
        if (fields.Count > 0)
        {
            injectFieldsInfo = InjectFieldsTemplate.CreateInjectFields(mainType, fields, (u) => usings.Add(u));
        }

        var injectPropertiesInfo = "";
        if (properties.Count > 0)
        {
            injectPropertiesInfo = InjectPropertiesTemplate.CreateProperties(mainType, properties, (u) => usings.Add(u));
        }

        var injectMethodsInfo = "";
        if (methods.Count > 0)
        {
            injectMethodsInfo = InjectMethodsTemplate.CreateInjectMethods(mainType, methods, (u) => usings.Add(u));
        }

        var ctorParametersSb = new StringBuilder();
        var constructor = mainType.GetInjectableConstructor();
        if (constructor != null)
        {
            var ctorParameters = constructor.Parameters;
            var sb = new StringBuilder();
            // ctorParameters.ToList().ForEach(p => sb.AppendLine(p.Name + ": " + p.Type.Name));
            // File.WriteAllText($"{mainType.Name}.txt", sb.ToString());
            foreach (var parameter in ctorParameters)
            {
                var parameterGetter = CreateMethodByCtorParameter(mainType, parameter, (u) => usings.Add(u));
                ctorParametersSb.AppendLine(parameterGetter);
            }
        }


        var usingsSb = new StringBuilder();
        foreach (var u in usings)
        {
            usingsSb.AppendLine($"using {u};");
        }

        var code = $$"""
namespace VContainer.Injectors;

using System.Collections.Generic;

{{usingsSb}}

public readonly struct {{mainType.Name}}Injector : IInjector
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

    private static string CreateMethodByCtorParameter(INamedTypeSymbol mainType, IParameterSymbol parameter, Action<string> addUsing)
    {
        var typeName = parameter.Type.GetTypeName();
        foreach (var geneticType in typeName.GenericTypes)
        {
            addUsing(geneticType.ContainingNamespace.ToDisplayString());
        }
        addUsing(parameter.ContainingNamespace.ToDisplayString());
        addUsing(parameter.Type.ContainingNamespace.ToDisplayString());
        var variable = parameter.Name.FirstCharToLower();
        var code = $$"""
                    private {{typeName}} Get{{parameter.Name.FirstCharToUpper()}}(IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{variable}} = ({{typeName}})objResolver.ResolveOrParameter(typeof ({{typeName}}), "{{typeName}}", parameters, typeof ({{mainType.GetTypeName()}}));
                        return {{variable}};
                    }
""";
        return code;
    }
}
