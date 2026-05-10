using System.IO.Compression;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class ZipArchiveHandler : IArchiveHandler
{  
    public List<string> GetAllPathsFromArchive(string archivePath, string? fileExtensionFilter = null)
    {
        using var archive = ZipFile.OpenRead(archivePath);

        return [.. archive.Entries
            .Where(e => !string.IsNullOrEmpty(e.Name)) // skip directories
            .Where(e => fileExtensionFilter is null ||
                string.Equals(Path.GetExtension(e.FullName), 
                    fileExtensionFilter.StartsWith('.') ? fileExtensionFilter : "." + fileExtensionFilter, 
                    StringComparison.OrdinalIgnoreCase))
            .Select(e => e.FullName)];
    }

    public void ExtractFiles(string archivePath, IEnumerable<(string internalPath, string targetPath)> fileMapping)
    {
        using var archive = ZipFile.OpenRead(archivePath);

        foreach (var (internalPath, targetPath) in fileMapping)
        {
            var entry = archive.GetEntry(internalPath);
            if (entry == null) continue;

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            entry.ExtractToFile(targetPath, overwrite: true);
        }
    }
}