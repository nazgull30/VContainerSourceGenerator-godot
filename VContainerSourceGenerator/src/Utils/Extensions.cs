namespace VContainerSourceGenerator.Utils;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class Extensions
{
    public static string FirstCharToLower(this string source) =>
        string.Concat(source[..1].ToLowerInvariant(), source.AsSpan(1));

    public static string FirstCharToUpper(this string source) =>
        string.Concat(source[..1].ToUpperInvariant(), source.AsSpan(1));

    public static string FromDotNetTypeToCSharpType(this string dotNetTypeName, bool isNull = false)
    {
        var cstype = "";
        var nullable = isNull ? "?" : "";
        var prefix = "System.";
        var typeName = dotNetTypeName.StartsWith(prefix) ? dotNetTypeName[prefix.Length..] : dotNetTypeName;

        cstype = typeName switch
        {
            "Boolean" => "bool",
            "Byte" => "byte",
            "SByte" => "sbyte",
            "Char" => "char",
            "Decimal" => "decimal",
            "Double" => "double",
            "Single" => "float",
            "Int32" => "int",
            "UInt32" => "uint",
            "Int64" => "long",
            "UInt64" => "ulong",
            "Object" => "object",
            "Int16" => "short",
            "UInt16" => "ushort",
            "String" => "string",
            _ => typeName,
        };
        return $"{cstype}{nullable}";
    }

    public static string FormatCode(this string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();
        var formattedRoot = root.NormalizeWhitespace();

        return formattedRoot.ToFullString();
    }

    public static string GetBaseTypeNameOfGeneric(this INamedTypeSymbol type)
    {
        return type.Name[..^2];
    }

    public static TypeName GetTypeName(this ITypeSymbol type)
    {
        var named = type as INamedTypeSymbol;
        return GetTypeName(named);
    }

    public static TypeName GetTypeName(this INamedTypeSymbol type)
    {
        string parameterTypeStr;
        var isNested = type.ContainingType != null;
        if (isNested)
        {
            parameterTypeStr = $"{type.ContainingType.Name}.{type.Name}";
        }
        else
        {
            var typeName = type.IsGenericType ? GetGenericTypeName(type) : type.Name;
            parameterTypeStr = type.IsPrimitiveType()
                ? type.ConvertToPrimitive()
                : typeName;
        }

        var genericTypes = type.IsGenericType ? type.TypeParameters : [];
        return new TypeName(parameterTypeStr, genericTypes);
    }

    private static string GetGenericTypeName(INamedTypeSymbol type)
    {
        var sb = new StringBuilder();
        var baseName = type.GetBaseTypeNameOfGeneric();
        sb.Append(baseName).Append('<');
        var genericArgs = type.TypeParameters;
        foreach (var arg in genericArgs)
        {
            sb.Append(arg.Name).Append(',');
        }
        if (genericArgs.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }
        sb.Append('>');
        return sb.ToString();
    }

    public static bool IsPrimitiveType(this ITypeSymbol type) => type.SpecialType.IsPrimitiveType();

    public static string ConvertToPrimitive(this ITypeSymbol type) => _universalTypes[type.Name];

    private static readonly Dictionary<string, string> _universalTypes = new()
        {
            {"SByte", "sbyte"},
            {"Byte", "byte"},
            {"Int16", "short"},
            {"UInt16", "ushort"},
            {"Int32", "int"},
            {"UInt32", "uint"},
            {"Int64", "long"},
            {"UInt64", "ulong"},
            {"Single", "float"},
            {"Double", "double"},
            {"Boolean", "bool"},
            {"Char", "char"},
            {"String", "string"},
            {"Object", "object"},
            {"Void", "void"}
        };

    public readonly struct TypeName
    {
        public readonly string Name;
        public readonly ImmutableArray<ITypeParameterSymbol> GenericTypes;

        public TypeName(string name, ImmutableArray<ITypeParameterSymbol> genericTypes)
        {
            Name = name;
            GenericTypes = genericTypes;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public static bool IsPrimitiveType(this SpecialType specialType)
    {
        return specialType is >= SpecialType.System_Boolean and <= SpecialType.System_UInt64;
    }

    public static List<IFieldSymbol> GetFields(this INamedTypeSymbol type)
    {
        return [.. type.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => !f.IsImplicitlyDeclared)];
    }

    public static List<IPropertySymbol> GetProperties(this INamedTypeSymbol type)
    {
        return [.. type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(f => !f.IsImplicitlyDeclared)];
    }

    public static List<IMethodSymbol> GetMethods(this INamedTypeSymbol type)
    {
        return [.. type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(f => !f.IsImplicitlyDeclared)];
    }
}
