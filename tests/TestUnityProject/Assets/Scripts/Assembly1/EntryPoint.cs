using UnityEngine;
using VContainer.Unity;

public class EntryPoint : IStartable
{
    private readonly IService1 _service1;
    private readonly IService2 _service2;

    public EntryPoint(IService1 service1, IService2 service2)
    {
        _service1 = service1;
        _service2 = service2;
    }

    public void Start()
    {
        Debug.Log("IService1 -> " + _service1.DoSomething());
        Debug.Log("IService2 -> " + _service2.DoSomething());
    }
}
