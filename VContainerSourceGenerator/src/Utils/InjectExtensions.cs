namespace VContainerSourceGenerator.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class InjectExtensions
{
    public static List<IFieldSymbol> GetInjectableFields(this INamedTypeSymbol typeSymbol)
    {
        var res = new List<IFieldSymbol>();
        while (typeSymbol != null && typeSymbol.SpecialType != SpecialType.System_Object)
        {
            var fields = typeSymbol.GetFields().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
            foreach (var fieldSymbol in fields)
            {
                var hasInject = fieldSymbol.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
                if (hasInject)
                {
                    res.Add(fieldSymbol);
                }
            }

            // Move to the base class
            typeSymbol = typeSymbol.BaseType;
        }
        return res;
    }

    public static List<IPropertySymbol> GetInjectableProperties(this INamedTypeSymbol typeSymbol)
    {
        var res = new List<IPropertySymbol>();
        while (typeSymbol != null && typeSymbol.SpecialType != SpecialType.System_Object)
        {
            var properties = typeSymbol.GetProperties().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
            foreach (var propertySymbol in properties)
            {
                var hasInject = propertySymbol.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
                if (hasInject)
                {
                    res.Add(propertySymbol);
                }
            }

            // Move to the base class
            typeSymbol = typeSymbol.BaseType;
        }
        return res;
    }

    public static List<IMethodSymbol> GetInjectableMethods(this INamedTypeSymbol typeSymbol)
    {
        var res = new List<IMethodSymbol>();
        while (typeSymbol != null && typeSymbol.SpecialType != SpecialType.System_Object)
        {
            var methods = typeSymbol.GetMethods().Where(f => f.DeclaredAccessibility is Accessibility.Public or Accessibility.Private or Accessibility.Protected).ToList();
            foreach (var method in methods)
            {
                var hasInject = method.GetAttributes().Where(f => f.AttributeClass.Name == "InjectAttribute").Any();
                if (hasInject)
                {
                    res.Add(method);
                }
            }

            // Move to the base class
            typeSymbol = typeSymbol.BaseType;
        }
        return res;
    }

    public static IMethodSymbol GetInjectableConstructor(this INamedTypeSymbol typeSymbol)
    {
        var constructors = typeSymbol.Constructors.Where(c => !c.IsStatic).ToList();
        if (constructors.Count == 0)
        {
            return null;
        }

        if (constructors.Count != 1)
        {
            throw new ArgumentException($"Should be at least only one constructor {typeSymbol.Name} for class {typeSymbol.Name}. We found: {constructors.Count} cto");
        }

        return constructors[0];
    }
}
