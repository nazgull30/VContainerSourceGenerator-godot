namespace VContainerSourceGenerator.Tests.Src.Models;

using Godot;
using VContainer;

public interface ILogger
{
    public void Log(string message);
}

[GenerateInjector]
public class PlayerLogger : ILogger
{
    public void Log(string message)
    {
        GD.Print($"PlayerLogger: {message}");
    }
}

[GenerateInjector]
public class EnemyLogger : ILogger
{
    public void Log(string message)
    {
        GD.Print($"EnemyLogger: {message}");
    }
}

[GenerateInjector]
public class DefaultLogger : ILogger
{
    public void Log(string message)
    {
        GD.Print(message);
    }
}
