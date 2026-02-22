namespace Extractor.Tests.Mock;

public class MockOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
{
    public T CurrentValue { get; }

    public MockOptionsMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public T Get(string name)
    {
        return CurrentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        throw new NotImplementedException();
    }
}
