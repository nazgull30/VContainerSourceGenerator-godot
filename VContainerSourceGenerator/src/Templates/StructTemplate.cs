namespace VContainerSourceGenerator.Templates;

using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class StructTemplate
{
    public static string Create(INamedTypeSymbol mainType)
    {
        var mainTypeName = mainType.IsGenericType ? mainType.GetBaseTypeNameOfGeneric() : mainType.Name;

        var fields = mainType.GetInjectableFields();
        var properties = mainType.GetInjectableProperties();
        var methods = mainType.GetInjectableMethods();

        var injectInfo = InjectTemplate.Create(fields.Count, properties.Count, methods.Count);
        var createInstanceInfo = CreateInstanceTemplate.CreateInstance(mainType);

        var statements = new StringBuilder();

        if (fields.Count > 0)
        {
            var injectFieldsInfo = InjectFieldsTemplate.CreateInjectFields(mainType, fields);
            statements.Append(injectFieldsInfo);
        }
        if (properties.Count > 0)
        {
            var injectPropertiesInfo = InjectPropertiesTemplate.CreateProperties(mainType, properties);
            statements.Append(injectPropertiesInfo);
        }
        if (methods.Count > 0)
        {
            var injectMethodsInfo = InjectMethodsTemplate.CreateInjectMethods(mainType, methods);
            statements.Append(injectMethodsInfo);
        }

        var constructor = mainType.GetInjectableConstructor();
        var ctorParameters = constructor.Parameters;
        foreach (var parameter in ctorParameters)
        {
            // foreach (var genericType in parameter.ParameterType.GetTypeName().GenericTypes)
            // {
            //     usingDirectives.AddUsingDirective(genericType.Namespace);
            // }

            var parameterGetter = CreateMethodByCtorParameter(mainType, parameter);
            statements.Append(parameterGetter);
            // if (parameterGetter.UsingDirective != null)
            // {
            //     usingDirectives.Add(parameterGetter.UsingDirective);
            // }
        }

        var code = $$"""
                    public void Inject(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        {{statements}}
                    }
""";

        return code;
    }

    private static string CreateMethodByCtorParameter(INamedTypeSymbol mainType, IParameterSymbol parameter)
    {
        var parameterTypeStr = parameter.Type.GetTypeName();
        var variable = parameter.Name.FirstCharToLower();
        var code = $$"""
                    private {{parameterTypeStr}} {{parameter.Name.FirstCharToUpper()}}(IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{variable}} = ({{parameterTypeStr}})objResolver.ResolveOrParameter(typeof ({{parameterTypeStr}}), "{{parameterTypeStr}}", parameters, typeof ({{mainType.GetTypeName()}}));
                        return {{variable}}
                    }
""";
        return code;
    }
}
