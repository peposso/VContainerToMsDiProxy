using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using VContainer;

namespace VContainerToMsDiProxy;

sealed class ServiceCollection : IServiceCollection
{
    List<ServiceDescriptor> services = new();
    IContainerBuilder builder;

    internal ServiceCollection(IContainerBuilder builder)
    {
        this.builder = builder;
    }

    ServiceDescriptor IList<ServiceDescriptor>.this[int index]
    {
        get => services[index];
        set => throw new NotSupportedException("This collection is read-only.");
    }

    int ICollection<ServiceDescriptor>.Count => services.Count;

    bool ICollection<ServiceDescriptor>.IsReadOnly => true;

    public void Add(ServiceDescriptor item)
    {
        RegisterService(item);
        services.Add(item);
    }

    void ICollection<ServiceDescriptor>.Clear()
        => throw new NotSupportedException("This collection is read-only.");

    bool ICollection<ServiceDescriptor>.Contains(ServiceDescriptor item)
        => services.Contains(item);

    void ICollection<ServiceDescriptor>.CopyTo(ServiceDescriptor[] array, int arrayIndex)
        => services.CopyTo(array, arrayIndex);

    IEnumerator<ServiceDescriptor> IEnumerable<ServiceDescriptor>.GetEnumerator()
        => services.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => services.GetEnumerator();

    int IList<ServiceDescriptor>.IndexOf(ServiceDescriptor item)
        => services.IndexOf(item);

    void IList<ServiceDescriptor>.Insert(int index, ServiceDescriptor item)
    {
        RegisterService(item);
        services.Insert(index, item);
    }

    bool ICollection<ServiceDescriptor>.Remove(ServiceDescriptor item)
        => throw new NotSupportedException("This collection is read-only.");

    void IList<ServiceDescriptor>.RemoveAt(int index)
        => throw new NotSupportedException("This collection is read-only.");

    void RegisterService(ServiceDescriptor item)
    {
        var lifetime = item.Lifetime switch
        {
            ServiceLifetime.Singleton => Lifetime.Singleton,
            ServiceLifetime.Scoped => Lifetime.Scoped,
            ServiceLifetime.Transient => Lifetime.Transient,
            _ => throw new NotSupportedException(),
        };

        if (item.ImplementationInstance != null)
        {
            builder.RegisterInstance(item.ImplementationInstance).As(item.ServiceType);
        }
        else if (item.ImplementationFactory != null)
        {
            builder.Register(new FactoryRegistrationBuilder(item.ImplementationFactory, item.ServiceType, lifetime));
        }
        else if (item.ImplementationType == null)
        {
            builder.Register(item.ServiceType, lifetime);
        }
        else
        {
            builder.Register(item.ImplementationType, lifetime).As(item.ServiceType);
        }
    }
}
