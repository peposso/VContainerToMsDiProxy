using System;
using VContainer;
using Microsoft.Extensions.DependencyInjection;

namespace VContainerToMsDiProxy;

public static class VContainerToMsDiAdaptorExtensions
{
    public static void ConfigureServiceCollection(this IContainerBuilder builder, Action<IServiceCollection> configure)
    {
        builder.Register<IServiceProvider>(x => new ServiceProvider(x), Lifetime.Scoped);

        var serviceCollection = new ServiceCollection(builder);
        configure(serviceCollection);
    }

    public static IServiceProvider ToServiceProvider(this IObjectResolver resolver)
    {
        return resolver.Resolve<IServiceProvider>();
    }
}
