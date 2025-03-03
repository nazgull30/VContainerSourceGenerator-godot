namespace VContainerSourceGenerator.Tests.Src.Models;

using System.Collections.Generic;
using Godot;
using VContainer;

[GenerateInjector]
public class PrinterHolder
{
    private readonly IReadOnlyList<IPrinter> _printers;

    public PrinterHolder(IReadOnlyList<IPrinter> printers)
    {
        _printers = printers;
    }

    public void Print()
    {
        GD.Print($"PlayerHolder -> Print, printers: {_printers.Count}");
        foreach (var printer in _printers)
        {
            printer.Print();
        }
    }
}
