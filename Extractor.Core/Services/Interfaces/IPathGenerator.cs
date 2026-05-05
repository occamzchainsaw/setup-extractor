using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IPathGenerator
{
    string GenerateRelativePath(PathTemplateContext ctx);
    string GenerateFullPath(PathTemplateContext ctx);
    string GenerateTemplateStringFromEnums(IEnumerable<PathElement> elements);
    List<PathElement> DeconstructTemplateElementsFromSettings();
}
