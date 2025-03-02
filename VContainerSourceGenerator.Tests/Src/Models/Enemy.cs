namespace VContainerSourceGenerator.Tests.Src.Models;

using VContainer;
using VContainer.Factory;

// [GenerateInjector]
public class Enemy
{
    private readonly float _speed;
    private readonly ILogger _logger;

    public Enemy(float speed, ILogger logger)
    {
        _speed = speed;
        _logger = logger;
    }

    public void Move()
    {
        _logger.Log($"Move with speed {_speed}");
    }

    // [GenerateInjector]
    public class Factory : PlaceholderFactory<float, Enemy>
    {

    }
}
