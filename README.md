# VContainerToMsDiProxy
VContainer.IContainerBuilder to Microsoft.Extensions.DependencyInjection.IServiceCollection proxy.

## Install

Package Manager > Add package from git URL...
```
https://github.com/peposso/VContainerToMsDiProxy?path=package
```

## Usage

```csharp
using VContainer;
using VContainer.Unity;
using VContainerToMsDiProxy;
using Microsoft.Extensions.DependencyInjection;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.ConfigureServiceCollection((IServiceCollection services) =>
        {
            services.AddScoped<IService1, Service1>();
        });
    }
}
```

## License

MIT
