namespace VContainerSourceGenerator.Tests.Src;

using VContainer;
using VContainer.Injectors;
using VContainer.Unity;
using VContainerSourceGenerator.Tests.Src.Models;
using VContainerSourceGenerator.Tests.Src.Models.Test;

public partial class RootInstaller : MonoInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        InjectorCache.SetInjectorsRepo(new InjectorRepo());

        builder.Register<PrinterOne>(Lifetime.Transient).AsImplementedInterfaces();
        builder.Register<PrinterTwo>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();
        builder.Register<PrinterThree>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();
        builder.Register<LogExample>(Lifetime.Singleton).AsSelf();
        builder.Register<PrinterHolder>(Lifetime.Singleton).AsSelf();

        builder.Register<InterfaceHolder>(Lifetime.Singleton).AsSelf();

        // builder.Register<FirstClass>(Lifetime.Singleton).WithParameter("First");
        // builder.Register<SecondClass>(Lifetime.Singleton).WithParameter("Second");
        // builder.Register<TestClass>(Lifetime.Singleton);
    }
}
