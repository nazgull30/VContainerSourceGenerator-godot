namespace VContainerSourceGenerator.Tests.Src;

using Godot;
using VContainer;

[GenerateInjector]
public class TestClass
{
    public TestClass(FirstClass firstClass, SecondClass secondClass)
    {
        FirstClass = firstClass;
        SecondClass = secondClass;
    }

    public FirstClass FirstClass { get; }
    public SecondClass SecondClass { get; }

    public void Print()
    {
        GD.Print(FirstClass.GetHashCode(), SecondClass.GetHashCode());
    }
}
