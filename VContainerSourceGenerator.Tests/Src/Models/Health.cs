namespace VContainerSourceGenerator.Tests.Src.Models;

using Godot;
using VContainer;

[GenerateInjector]
public class Health : BasicHealth
{
    public int Min;
    public int Max;

    public Health(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override string ToString()
    {
        return $"{nameof(Min)}: {Min}, {nameof(Max)}: {Max}";
    }
}

public class BasicHealth
{
    [Inject]
    public void SetTestPrivateInjection()
    {
        GD.Print("SetTestPrivateInjection");
    }
}

public class TestPrivateInjection
{

}
