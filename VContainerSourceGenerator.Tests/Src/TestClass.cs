namespace VContainerSourceGenerator.Tests.Src;

using VContainer;

[GenerateInjector]
public class TestClass
{
    private readonly int _i;

    public TestClass(int i)
    {
        _i = i;
    }

    public int I { get; set; }
}
