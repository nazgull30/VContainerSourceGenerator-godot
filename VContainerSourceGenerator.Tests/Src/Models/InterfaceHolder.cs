namespace VContainerSourceGenerator.Tests.Src.Models;

using System.Collections.Generic;
using Godot;
using VContainer;

public interface IMyInterface
{

}

[GenerateInjector]
public class InterfaceHolder
{
    private readonly IList<IMyInterface> _myInterfaces;

    public InterfaceHolder(IList<IMyInterface> myInterfaces)
    {
        _myInterfaces = myInterfaces;
    }

    public void PrintCount()
    {
        GD.Print("My Interfaces: " + _myInterfaces.Count);
    }
}
