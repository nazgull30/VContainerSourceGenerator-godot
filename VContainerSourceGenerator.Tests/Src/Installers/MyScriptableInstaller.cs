namespace VContainerSourceGenerator.Tests.Src.Installers;

using Godot;
using VContainer;
using VContainer.Unity;
using VContainerSourceGenerator.Tests.Src.Models;

[GlobalClass]
[GenerateInjector]
public partial class MyScriptableInstaller : ScriptableObjectInstaller
{
    [Inject] private LogExample _logExample;

    public override void Install(IContainerBuilder builder)
    {
        _logExample.Log();

        builder.RegisterInstance(new Health { Min = 10, Max = 100 });
        builder.RegisterFactory<float, Enemy, Enemy.TestFactory>();
        builder.Register<Player>(Lifetime.Singleton).AsSelf().WithParameter(2f);

        builder.Register<SpawnService>(Lifetime.Singleton).AsImplementedInterfaces();

        // builder.RegisterPool<Player, PlayerPool, IPlayerPool>();

        InstallLoggers(builder);
    }

    private void InstallLoggers(IContainerBuilder builder)
    {
        builder.Register<DefaultLogger>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.Register<PlayerLogger>(Lifetime.Singleton).AsImplementedInterfaces().WhenInjectedInto<Player>();
        builder.Register<EnemyLogger>(Lifetime.Singleton).AsImplementedInterfaces().WhenInjectedInto<Enemy>();
    }

}
