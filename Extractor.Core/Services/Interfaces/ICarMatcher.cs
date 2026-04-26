using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface ICarMatcher
{
    CarMatchResult TryMatchCar(string archiveEntryPath);
}