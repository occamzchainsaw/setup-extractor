using Microsoft.Extensions.Options;

namespace Extractor.Tests.Mock;

public class MockOptionsMonitor<T> : IOptionsMonitor<T> where T : class, new()
{ 
    public MockOptionsMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public T CurrentValue { get; }

    public T Get(string? name) => CurrentValue;

    public IDisposable OnChange(Action<T, string> listener) => new DummyDisposable();

    private class DummyDisposable : IDisposable { public void Dispose() { } }
}
