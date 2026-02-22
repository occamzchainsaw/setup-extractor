using System.Text.RegularExpressions;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using FuzzySharp;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class DefaultTrackMatcher : ITrackMatcher
{
    private readonly IOptionsMonitor<TracksData> _tracksMonitor;
    private readonly IOptionsMonitor<CoreConfig> _configMonitor;

    public DefaultTrackMatcher(
        IOptionsMonitor<TracksData> tracksMonitor,
        IOptionsMonitor<CoreConfig> configMonitor
    )
    {
        _tracksMonitor = tracksMonitor;
        _configMonitor = configMonitor;
    }

    public TrackMatchResult TryMatchTrack(string path)
    {
        var tracks = _tracksMonitor.CurrentValue.Tracks;
        var minConfidenceScore = Math.Max(_configMonitor.CurrentValue.MinConfidenceScore, 45);

        if (tracks is null || !tracks.Any())
            return TrackMatchResult.NotFound();

        string cleanPath = NormalizeString(path);

        TrackMatchResult bestMatch = TrackMatchResult.NotFound();

        foreach (var track in tracks)
        {
            int cardinalScore = Fuzz.PartialRatio(NormalizeString(track.Cardinal), cleanPath);
            CheckScore(track.Cardinal, track.Cardinal, cardinalScore); // cardinal is an alias in itself

            foreach (var alias in track.Aliases)
            {
                int aliasScore = Fuzz.PartialRatio(NormalizeString(alias), cleanPath);
                CheckScore(track.Cardinal, alias, aliasScore);
            }
        }

        return bestMatch;

        void CheckScore(string cardinal, string matchedText, int score)
        {
            if (score >= minConfidenceScore && score > bestMatch.ConfidenceScore)
            {
                bestMatch = new TrackMatchResult
                {
                    Success = true,
                    CardinalName = cardinal,
                    MatchedAlias = matchedText,
                    ConfidenceScore = score,
                };
            }
        }
    }

    private static string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string s = input.ToLowerInvariant();
        s = Regex.Replace(s, "[^a-z0-9]", "");

        return s;
    }
}
