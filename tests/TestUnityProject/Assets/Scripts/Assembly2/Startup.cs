using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// Assembly2 -> noEngineReferences: true
public class Startup
{
    public void Configure(IServiceCollection services)
    {
        services.AddScoped<IService1, Service1>();
        services.AddScoped<IService2>(_ => new Service2("I'm Service2."));
    }
}
