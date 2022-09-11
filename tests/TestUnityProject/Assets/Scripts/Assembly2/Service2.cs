internal class Service2 : IService2
{
    private readonly string message;

    internal Service2(string message)
    {
        this.message = message;
        this.message = message;
    }

    public string DoSomething()
    {
        return message;
    }
}
