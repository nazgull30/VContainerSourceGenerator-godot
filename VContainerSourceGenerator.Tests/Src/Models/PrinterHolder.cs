namespace VContainerSourceGenerator.Tests.Src.Models;

using System.Collections.Generic;
using Godot;

public class PrinterHolder
{
    private readonly IList<IPrinter> _printers;

    public PrinterHolder(IList<IPrinter> printers)
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
