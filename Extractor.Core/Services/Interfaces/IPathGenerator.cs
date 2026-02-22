using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface IPathGenerator
{
    string GeneratePath(PathTemplateContext ctx);
    string UpdateTemplateFromEnums(IEnumerable<PathElement> elements);
}
