using System;
using System.IO;
using System.Threading.Tasks;
using Extractor.Core.Model;
using Extractor.Core.Services;
using Extractor.Tests.Mock;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Extractor.Tests;

[TestFixture]
public class CarMatcherTests
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
    public void ShouldMatchExactFolderName()
    {
        var carFolder = "porsche9922cup";
        Directory.CreateDirectory(Path.Combine(_tempDir, carFolder));

        var options = CreateOptions(_tempDir, 90);
        var matcher = new CarMatcher(options);

        var result = matcher.TryMatchComponentFromPath(@"setup_test/porsche9922cup/nurburgring_combined/HYMO_PCUP_26S2_Nords/somefile.sto");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.True, "Expected a successful match for exact folder name");
            Assert.That(result.CardinalName, Is.EqualTo(carFolder));
            Assert.That(result.ConfidenceScore, Is.EqualTo(100));
            Assert.That(result.MatchedAlias, Is.EqualTo(carFolder));
        }
    }

    [Test]
    public void ShouldNotMatchUnknownCar()
    {
        var carFolder = "porsche992rgt3";
        Directory.CreateDirectory(Path.Combine(_tempDir, carFolder));

        var options = CreateOptions(_tempDir, 90);
        var matcher = new CarMatcher(options);

        var result = matcher.TryMatchComponentFromPath("setup_test/porsche9922cup/nurburgring_combined/HYMO_PCUP_26S2_Nords/somefile.sto");

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
}