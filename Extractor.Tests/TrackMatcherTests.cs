using System.Text.Json;
using Extractor.Core.Model;
using Extractor.Core.Services;
using Extractor.Tests.Mock;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Extractor.Tests;

[TestFixture]
public class TrackMatcherTests
{
    private string _tempDir = string.Empty;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }
    
    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }
        catch
        {
            // best-effort cleanup
        }
    }

    [Test]
    public void ShouldMatchTrack()
    {
        var options = CreateOptions(_tempDir);
        var tracksOptions = CreateTracksOptions();
        var matcher = new TrackMatcher(tracksOptions, options);

        var result =
            matcher.TryMatchComponentFromPath(@"setup_test/porsche9922cup/nurburgring_combined/HYMO_PCUP_26S2_Nords/somefile.sto");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.True, "Expected a successful match for exact track name");
            Assert.That(result.CardinalName, Is.EqualTo("Nurburgring Combined"));
            Assert.That(result.ConfidenceScore, Is.GreaterThan(75));
        }
    }

    [Test]
    public void ShouldNotMatchTrack()
    {
        var options = CreateOptions(_tempDir);
        var tracksOptions = CreateTracksOptions();
        var matcher = new TrackMatcher(tracksOptions, options);

        var result =
            matcher.TryMatchComponentFromPath(@"setup_test/porsche9922cup/somewhatevertracknoonehaseverheardof/HYMO_PCUP_26S2_Nords/somefile.sto");
        
        Assert.That(result.Success, Is.False);
    }
    
    private static IOptionsMonitor<CoreConfig> CreateOptions(string basePath, int minConfidence = 0)
    {
        return new MockOptionsMonitor<CoreConfig>(new CoreConfig
        {
            SetupsBasePath = basePath,
            MinConfidenceScore = minConfidence
        });
    }

    private static IOptionsMonitor<TracksData> CreateTracksOptions()
    {
        var rawJson = File.ReadAllText("/home/paul/dev/setup-extractor/tracksData.json");
        var tracksData = JsonSerializer.Deserialize<TracksData>(rawJson);
        return new MockOptionsMonitor<TracksData>(tracksData!);
    }
}
