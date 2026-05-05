using Extractor.Core.Model;

namespace Extractor.Gui.Models;

public class PathElementDto
{
    public PathElement Value { get; set; }
}

public static class PathElementExtensions
{
    extension(PathElement element)
    {
        public PathElementDto ToDto()
        {
            return new PathElementDto { Value = element };
        }
    }
}