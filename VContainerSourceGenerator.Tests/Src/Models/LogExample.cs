namespace VContainerSourceGenerator.Tests.Src.Models;

using Godot;
using VContainer;

[GenerateInjector]
public class LogExample
{
    public void Log()
    {
        GD.Print("LogExample");
    }
}
