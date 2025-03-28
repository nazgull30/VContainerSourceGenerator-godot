namespace VContainerSourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using VContainerSourceGenerator.Utils;

public static class InjectMethodsTemplate
{
    public static string CreateInjectMethods(INamedTypeSymbol mainType, List<IMethodSymbol> methods, Action<string> addUsing)
    {
        AddUsings(methods, addUsing);
        var statements = new StringBuilder();

        foreach (var methodInfo in methods)
        {
            var methodStatements = CreateStatementsForOneMethod(mainType, methodInfo);
            statements.Append(methodStatements);
        }


        var code = $$"""
                    private void InjectMethods(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        var {{mainType.GetVariableName()}} = ({{mainType.GetTypeName()}}) instance;
                        {{statements}}
                    }
""";

        return code;
    }

    private static StringBuilder CreateStatementsForOneMethod(INamedTypeSymbol mainType, IMethodSymbol methodInfo)
    {
        var parameters = methodInfo.Parameters;
        var statements = new StringBuilder();

        foreach (var parameter in parameters)
        {
            var parameterStatements = CreateStatementsForOneParameter(mainType, parameter);
            statements.AppendLine(parameterStatements);
        }

        if (methodInfo.DeclaredAccessibility == Accessibility.Public)
        {
            var sb = new StringBuilder();
            foreach (var parameter in parameters)
            {
                sb.Append($"({parameter.Type.GetTypeName()}){parameter.Name}").Append(',');
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
                sb.Append($"{variable}").Append(',');
            }

            if (parameters.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            statements.AppendLine($"InjectorUtils.InjectNotPublicMethod(instance, \"{methodInfo.Name}\"{sb});");
        }

        return statements;
    }

    private static string CreateStatementsForOneParameter(INamedTypeSymbol mainType, IParameterSymbol parameter)
    {
        var variable = parameter.Name.FirstCharToLower();
        var resolveStr = $"var {variable} = objResolver.ResolveOrParameter(typeof({parameter.Type.GetTypeName()}), \"{parameter.Type.GetTypeName()}\", parameters, typeof({mainType.GetTypeName()}));";
        return resolveStr;
    }

    private static void AddUsings(List<IMethodSymbol> methods, Action<string> addUsing)
    {
        foreach (var method in methods)
        {
            foreach (var parameter in method.Parameters)
            {
                addUsing(parameter.ContainingNamespace.ToDisplayString());
                var paramTypeName = parameter.Type.GetTypeName();
                foreach (var geneticType in paramTypeName.GenericTypes)
                {
                    addUsing(geneticType.ContainingNamespace.ToDisplayString());
                }
            }
            addUsing(method.ReturnType.ContainingNamespace.ToDisplayString());
        }
    }
}
