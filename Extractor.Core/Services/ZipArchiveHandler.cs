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
}