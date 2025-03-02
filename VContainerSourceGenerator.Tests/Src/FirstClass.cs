namespace VContainerSourceGenerator.Tests.Src;

using VContainer;

[GenerateInjector]
public class FirstClass
{
    public FirstClass(int name)
    {
        Name = name;
    }

    public int Name { get; }
}

[GenerateInjector]
public class SecondClass
{
    public SecondClass(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
