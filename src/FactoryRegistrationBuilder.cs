using VContainer;

namespace VContainerToMsDiProxy;

sealed class FactoryRegistrationBuilder : RegistrationBuilder
{
    private readonly Func<IServiceProvider, object> factory;
    private readonly Type serviceType;
    private readonly Lifetime lifetime;

    public FactoryRegistrationBuilder(Func<IServiceProvider, object> factory, Type serviceType, Lifetime lifetime)
        : base(serviceType, lifetime)
    {
        this.factory = factory;
        this.serviceType = serviceType;
        this.lifetime = lifetime;
    }

    public override Registration Build()
    {
        var provider = new FactoriedInstanceProvider(factory);
        return new Registration(serviceType, lifetime, new[] { serviceType }, provider);
    }
}
