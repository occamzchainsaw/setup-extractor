using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IPathGenerator
{
    string GenerateRelativePath(PathTemplateContext ctx);
    string GenerateFullPath(PathTemplateContext ctx);
    string UpdateTemplateFromEnums(IEnumerable<PathElement> elements);
}
