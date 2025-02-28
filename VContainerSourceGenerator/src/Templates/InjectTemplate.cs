namespace VContainerSourceGenerator.Templates;

using System.Text;

public static class InjectTemplate
{
    public static string Create(int fieldCount, int propertyCount, int methodCount)
    {
        var statements = new StringBuilder();

        if (fieldCount > 0)
        {
            statements.AppendLine("InjectFields(instance, objResolver, parameters);");
        }
        if (propertyCount > 0)
        {
            statements.AppendLine("InjectProperties(instance, objResolver, parameters);");
        }
        if (methodCount > 0)
        {
            statements.AppendLine("InjectMethods(instance, objResolver, parameters);");
        }

        var code = $$"""
                    public void Inject(object instance, IObjectResolver objResolver, IReadOnlyList<IInjectParameter> parameters)
                    {
                        {{statements}}
                    }
""";

        return code;
    }
}
