namespace SaveGameSync.Core.Models;

public sealed class SaveFileCandidate
{
    public required string Name { get; init; }

    public required string FilePath { get; init; }

    public long Size { get; init; }

    public bool IsSelected { get; set; }
}
