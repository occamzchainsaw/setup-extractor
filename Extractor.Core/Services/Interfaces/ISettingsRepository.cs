using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface ISettingsRepostory
{
    void SaveSettings(CoreConfig config);
    CoreConfig? ReadSettings();
}
