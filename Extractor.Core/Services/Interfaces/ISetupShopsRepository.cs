using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface ISetupShopsRepository
{
    void SaveSetupShops(SetupShopsData setupShopsData);
    SetupShopsData? ReadSetupShops();
}