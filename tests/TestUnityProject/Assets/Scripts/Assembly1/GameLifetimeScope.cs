using VContainer;
using VContainer.Unity;
using VContainerToMsDiProxy;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.ConfigureServiceCollection(x => new Startup().Configure(x));
        builder.RegisterEntryPoint<EntryPoint>();
    }
}
