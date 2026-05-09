using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class PathComposer(IOptionsMonitor<CoreConfig> configMonitor) : IPathComposer
{
    public string GenerateRelativePath(PathTemplateContext ctx, string fileName)
    {
        var template = configMonitor.CurrentValue.PathTemplate;

        return Path.Combine(
            ctx.Car, 
            template.Replace($"{Enum.GetName(PathElement.Track)}", ctx.Track)
                .Replace($"{Enum.GetName(PathElement.SeasonAndWeek)}", ctx.SeasonAndWeek)
                .Replace($"{Enum.GetName(PathElement.Season)}", ctx.Season)
                .Replace($"{Enum.GetName(PathElement.SetupShop)}", ctx.SetupShop),
            fileName);
    }

    public string GenerateFullPath(PathTemplateContext ctx, string fileName)
    {
        if (string.IsNullOrEmpty(configMonitor.CurrentValue.SetupsBasePath))
            return string.Empty;

        return Path.Combine(configMonitor.CurrentValue.SetupsBasePath, GenerateRelativePath(ctx, fileName));
    }

    public string GenerateTemplateStringFromEnums(IEnumerable<PathElement> elements)
    {
        var parts = elements
            .Select(e =>
                e switch
                {
                    PathElement.Track => $"{Enum.GetName(PathElement.Track)}",
                    PathElement.Season => $"{Enum.GetName(PathElement.Season)}",
                    PathElement.SeasonAndWeek => $"{Enum.GetName(PathElement.SeasonAndWeek)}",
                    PathElement.SetupShop => $"{Enum.GetName(PathElement.SetupShop)}",
                    _ => string.Empty,
                }
            )
            .Where(s => !string.IsNullOrWhiteSpace(s));

        return Path.Combine([.. parts]);
    }

    public List<PathElement> DeconstructTemplateElementsFromSettings(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return [];
        
        var separator = Path.DirectorySeparatorChar;
        var rawElements = template.Split(separator);
        var allElements = Enum.GetValues<PathElement>();
        return rawElements
            .Select(rawElement => allElements
                .First(e => Enum.GetName(e)!.Equals(rawElement, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
    }
}
