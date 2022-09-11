using System.Runtime.CompilerServices;
using VContainer;

namespace VContainerToMsDiProxy;

sealed class FactoriedInstanceProvider : IInstanceProvider
{
    private readonly Func<IServiceProvider, object> factory;

    public FactoriedInstanceProvider(Func<IServiceProvider, object> factory)
    {
        this.factory = factory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object SpawnInstance(IObjectResolver resolver)
    {
        var serviceProvider = resolver.Resolve<IServiceProvider>();
        return factory(serviceProvider);
    }
}
