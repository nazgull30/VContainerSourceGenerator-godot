namespace VContainerSourceGenerator.Tests.Src.Models;

using Godot;
using VContainer;
using VContainerSourceGenerator.Tests.Src.Models.Test;

// [GenerateInjector]
public class Player : IMyInterface
{
    private readonly float _speed;
    [Inject] private Health _health;
    private readonly ILogger _logger;

    [Inject] private IPrinterOne _printerOneOne;

    private IPrinterOne _printerOne;

    private PrinterTwo _printerTwo;

    [Inject] public Enemy.TestFactory _enemyFactoryOne;

    [Inject] public Enemy.TestFactory EnemyFactoryOne { set; get; }

    [Inject]
    protected void SetPrintOne(IPrinterOne printerOne)
    {
        _printerOne = printerOne;
    }

    [Inject]
    public void SetPrintTwo(PrinterTwo printerTwo)
    {
        _printerTwo = printerTwo;
    }

    [Inject] private PrinterTwo PrinterTwo { set; get; }

    [Inject]
    public PrinterThree PrinterThree { set; get; }

    public Player(float speed, ILogger logger)
    {
        _speed = speed;
        _logger = logger;
        GD.Print("Player");
    }

    public void Move()
    {
        _logger.Log($"Move with speed {_speed}, health: {_health}");

        _printerOne.Print();
        _printerOneOne.Print();
        _printerTwo.Print();
        PrinterThree.Print();
    }
}
