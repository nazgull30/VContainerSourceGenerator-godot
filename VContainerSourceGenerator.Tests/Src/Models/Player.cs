namespace VContainerSourceGenerator.Tests.Src.Models;

using VContainer;

// [GenerateInjector]
public class Player : IMyInterface
{
    private readonly float _speed;
    private readonly Health _health;
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

    public Player(float speed, Health health, ILogger logger)
    {
        _speed = speed;
        _health = health;
        _logger = logger;
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
