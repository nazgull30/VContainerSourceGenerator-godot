namespace VContainerSourceGenerator.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class InjectExtensions
{
    public static List<IFieldSymbol> GetInjectableFields(this INamedTypeSymbol baseType)
    {
        var res = new List<IFieldSymbol>();
        var fields = baseType.GetFields().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
        foreach (var fieldInfo in fields)
        {
            var hasInject = fieldInfo.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
            if (hasInject)
            {
                res.Add(fieldInfo);
            }
        }
        return res;
    }

    public static List<IPropertySymbol> GetInjectableProperties(this INamedTypeSymbol baseType)
    {
        var res = new List<IPropertySymbol>();
        var properties = baseType.GetProperties().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
        foreach (var propertySymbol in properties)
        {
            var hasInject = propertySymbol.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
            if (hasInject)
            {
                res.Add(propertySymbol);
            }
        }
        return res;
    }

    public static List<IMethodSymbol> GetInjectableMethods(this INamedTypeSymbol baseType)
    {
        var res = new List<IMethodSymbol>();
        var methods = baseType.GetMethods().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
        foreach (var methodSymbol in methods)
        {
            var hasInject = methodSymbol.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
            if (hasInject)
            {
                res.Add(methodSymbol);
            }
        }
        return res;
    }

    public static IMethodSymbol GetInjectableConstructor(this INamedTypeSymbol typeSymbol)
    {
        var constructors = typeSymbol.Constructors.Where(c => !c.IsStatic).ToList();
        if (constructors.Count != 1)
        {
            throw new ArgumentException($"Should be only one constructor {typeSymbol.Name}");
        }

        return constructors[0];
    }
}
