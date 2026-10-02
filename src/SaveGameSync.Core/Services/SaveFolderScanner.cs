using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Services;

public sealed class SaveFolderScanner
{
    public IReadOnlyList<SaveFileCandidate> Scan(
        string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentException(
                "Save folder path cannot be empty.",
                nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException(
                $"Save folder '{folderPath}' was not found.");
        }

        return Directory
            .EnumerateFiles(
                folderPath,
                "*",
                SearchOption.TopDirectoryOnly)
            .Select(filePath => new SaveFileCandidate
            {
                Name = Path.GetFileName(filePath),
                FilePath = filePath,
                Size = new FileInfo(filePath).Length,
                IsSelected = false
            })
            .OrderBy(candidate => candidate.Name)
            .ToList();
    }
}