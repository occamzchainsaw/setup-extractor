using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class DefaultPathGenerator : IPathGenerator
{
    private readonly IOptionsMonitor<CoreConfig> _options;

    public DefaultPathGenerator(IOptionsMonitor<CoreConfig> options)
    {
        _options = options;
    }

    public string GeneratePath(PathTemplateContext ctx)
    {
        var template = _options.CurrentValue.PathTemplate;

        return template
            .Replace($"{Enum.GetName(PathElement.Track)}", ctx.Track)
            .Replace($"{Enum.GetName(PathElement.Season)}", ctx.Season)
            .Replace($"{Enum.GetName(PathElement.Season)}", ctx.SeasonAndWeek)
            .Replace($"{Enum.GetName(PathElement.Season)}", ctx.SetupShop);
    }

    public string UpdateTemplateFromEnums(IEnumerable<PathElement> elements)
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
}
