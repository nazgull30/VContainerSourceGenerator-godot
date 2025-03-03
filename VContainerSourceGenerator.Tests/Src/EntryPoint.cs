namespace VContainerSourceGenerator.Tests.Src;

using Godot;
using VContainer;
using VContainer.Unity;
using VContainerSourceGenerator.Tests.Src.Models;

[GenerateInjector]
public partial class EntryPoint : Node, IInitializable
{
    [Inject] private Player _player;
    [Inject] private PrinterHolder _printerHolder;

    public void Initialize()
    {
        _printerHolder.Print();
        _player.Move();
    }
}
