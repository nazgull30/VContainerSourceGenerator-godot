namespace VContainerSourceGenerator.Tests.Src.Models;

using VContainer;
using VContainer.Runtime;

public interface ISpawnService<T1, T2>
{

}

// [GenerateInjector]
public class SpawnService : ISpawnService<Health, IExternalTypeRetriever>
{

}

// [GenerateInjector]
public class SpawnServiceGeneric<Health, Player>
{

}
