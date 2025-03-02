namespace VContainerSourceGenerator.Tests.Src.Models;

public class Health
{
    public int Min;
    public int Max;

    public override string ToString()
    {
        return $"{nameof(Min)}: {Min}, {nameof(Max)}: {Max}";
    }
}
