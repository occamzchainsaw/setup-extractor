using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IPathContextComposer
{
    PathTemplateContext ComposePathTemplateContext(string archivePath, string season, string week);
}