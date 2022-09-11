using VContainer;

namespace VContainerToMsDiProxy;

sealed class ServiceProvider : IServiceProvider
{
    private readonly IObjectResolver resolver;

    internal ServiceProvider(IObjectResolver resolver)
    {
        this.resolver = resolver;
    }

    public object GetService(Type serviceType)
    {
        return resolver.Resolve(serviceType);
    }
}
