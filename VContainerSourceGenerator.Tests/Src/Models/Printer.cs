namespace VContainerSourceGenerator.Tests.Src.Models;

using Godot;
using VContainer;

public interface IPrinter
{
    public void Print();
}

public interface IPrinterOne : IPrinter
{
}

// [GenerateInjector]
public class PrinterOne : IPrinterOne
{
    public void Print()
    {
        GD.Print("PRINT ONE");
    }
}

// [GenerateInjector]
public class PrinterTwo : IPrinter
{
    public void Print()
    {
        GD.Print("PRINT TWO");
    }
}

// [GenerateInjector]
public class PrinterThree : IPrinter
{
    public void Print()
    {
        GD.Print("PRINT TREE");
    }
}
