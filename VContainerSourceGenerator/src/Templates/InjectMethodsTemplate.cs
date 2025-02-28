namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VContainerSourceGenerator.Utils;

public static class InjectMethodsTemplate
{
    public static string CreateInjectMethods(Type mainType, List<MethodInfo> methods)
    {
        var statements = new StringBuilder();

        foreach (var methodInfo in methods)
        {
            var methodStatements = CreateStatementsForOneField(mainType, methodInfo);
            statements.Append(methodStatements);
        }


        var code = $$"""
                    private void InjectMethods(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{mainType.GetTypeName().Name.FirstCharToLower()}} = ({{mainType.GetTypeName()}}) instance;
                        {{statements}}
                    }
""";

        return code;
    }

    private static StringBuilder CreateStatementsForOneField(Type mainType, MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        var statements = new StringBuilder();

        foreach (var parameter in parameters)
        {
            var parameterStatements = CreateStatementsForOneParameter(mainType, parameter);
            statements.AppendLine(parameterStatements);
        }

        if (methodInfo.IsPublic)
        {
            var sb = new StringBuilder();
            foreach (var parameter in parameters)
            {
                sb.Append($"({parameter.ParameterType.GetTypeName()}){parameter.Name}").Append(',');
            }

            if (parameters.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            statements.AppendLine($"{mainType.Name.FirstCharToLower()}.{methodInfo.Name}({sb});");
        }
        else
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append(", ");
            }
            foreach (var parameter in parameters)
            {
                var variable = parameter.Name.FirstCharToLower();
                sb.Append($"{variable}").Append(",");
            }

            if (parameters.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            statements.AppendLine($"InjectorUtils.InjectNotPublicMethod(instance, \"{methodInfo.Name}\"{sb});");
        }

        return statements;
    }

    private static string CreateStatementsForOneParameter(Type mainType, ParameterInfo parameter)
    {
        var variable = parameter.Name.FirstCharToLower();
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({parameter.ParameterType.GetTypeName()}, \"{parameter.ParameterType.GetTypeName()}\", parameters, typeof({mainType.Name}));";
        return resolveStr;
    }
}
