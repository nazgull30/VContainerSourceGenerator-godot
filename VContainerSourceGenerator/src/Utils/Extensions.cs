namespace VContainerSourceGenerator.Utils;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class Extensions
{
    public static bool HasAttribute<T>(this MemberInfo type)
        where T : Attribute
        => type.GetCustomAttribute<T>() != null;

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

    public static string GetBaseTypeNameOfGeneric(this Type type)
    {
        return type.Name[..^2];
    }

    public static TypeName GetTypeName(this Type type)
    {
        string parameterTypeStr;
        var isNested = type.IsNested;
        if (isNested)
        {
            parameterTypeStr = $"{type.DeclaringType.Name}.{type.Name}";
        }
        else
        {
            var typeName = type.IsGenericType ? GetGenericTypeName(type) : type.Name;
            parameterTypeStr = type.IsPrimitiveType()
                ? type.ConvertToPrimitive()
                : typeName;
        }

        var genericTypes = type.IsGenericType ? type.GetGenericArguments() : Type.EmptyTypes;
        return new TypeName(parameterTypeStr, genericTypes);
    }

    private static string GetGenericTypeName(Type type)
    {
        var sb = new StringBuilder();
        var baseName = type.GetBaseTypeNameOfGeneric();
        sb.Append(baseName).Append('<');
        var genericArgs = type.GetGenericArguments();
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

    public static bool IsPrimitiveType(this Type type) => _universalTypes.ContainsKey(type.Name);

    public static string ConvertToPrimitive(this Type type) => _universalTypes[type.Name];

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
        public readonly Type[] GenericTypes;

        public TypeName(string name, Type[] genericTypes)
        {
            Name = name;
            GenericTypes = genericTypes;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
