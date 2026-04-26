using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IComponentMatcher<T> where T : IMatchResult
{
    T TryMatchComponentFromPath(string path);
}