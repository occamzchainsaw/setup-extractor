using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IPathComposer
{
    string GenerateRelativePath(PathTemplateContext ctx);
    string GenerateFullPath(PathTemplateContext ctx, string fileName);
    string GenerateTemplateStringFromEnums(IEnumerable<PathElement> elements);
    List<PathElement> DeconstructTemplateElementsFromSettings(string template);
}
