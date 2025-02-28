namespace VContainerSourceGenerator.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class InjectExtensions
    {
        public static List<FieldInfo> GetInjectableFields(this Type baseType)
        {
            var res = new List<FieldInfo>();
            var fields = baseType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var data = fieldInfo.GetCustomAttributesData();
                var selected = data.Where(x => x.AttributeType.Name == "InjectAttribute").Select(x => x).Count();
                if (selected > 0)
                {
                    res.Add(fieldInfo);
                }
            }
            return res;
        }

        public static List<PropertyInfo> GetInjectableProperties(this Type baseType)
        {
            var res = new List<PropertyInfo>();
            var properties = baseType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                var data = propertyInfo.GetCustomAttributesData();
                var selected = data.Where(x => x.AttributeType.Name == "InjectAttribute").Select(x => x).Count();
                if (selected > 0)
                {
                    res.Add(propertyInfo);
                }
            }
            return res;
        }

        public static List<MethodInfo> GetInjectableMethods(this Type baseType)
        {
            var res = new List<MethodInfo>();
            var methods = baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                var data = methodInfo.GetCustomAttributesData();
                var selected = data.Where(x => x.AttributeType.Name == "InjectAttribute").Select(x => x).Count();
                if (selected > 0)
                {
                    res.Add(methodInfo);
                }
            }
            return res;
        }

        public static ConstructorInfo GetInjectableConstructor(this Type baseType)
        {
            var constructors = baseType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (constructors.Length != 1)
            {
                throw new ArgumentException($"Should be only one constructor {baseType.Name}");
            }

            return constructors[0];
        }
    }
}
