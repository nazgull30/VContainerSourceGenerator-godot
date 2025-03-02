namespace VContainerSourceGenerator.Tests.Src;

using Godot;
using VContainer;
using VContainer.Unity;

[GenerateInjector]
public partial class EntryPoint : Node, IInitializable
{
    [Inject] private TestClass _testClass;

    public void Initialize()
    {
        _testClass.Print();
    }
}
