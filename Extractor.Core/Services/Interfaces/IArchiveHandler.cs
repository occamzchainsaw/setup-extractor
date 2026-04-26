namespace Extractor.Core.Services.Interfaces;

public interface IArchiveHandler
{
    List<string> GetAllPathsFromArchive(string archivePath, string? fileExtensionFilter = null);
}