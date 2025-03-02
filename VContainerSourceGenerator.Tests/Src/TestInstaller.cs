namespace VContainerSourceGenerator.Tests.Src;

using VContainer;
using VContainer.Injectors;
using VContainer.Unity;

public partial class TestInstaller : MonoInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        builder.Register<FirstClass>(Lifetime.Singleton).WithParameter("First");
        builder.Register<SecondClass>(Lifetime.Singleton).WithParameter("Second");
        builder.Register<TestClass>(Lifetime.Singleton);

        InjectorCache.SetInjectorsRepo(new InjectorRepo());

    }
}
