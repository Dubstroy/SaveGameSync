namespace SaveGameSync.Core.Models;

public enum SaveSyncStatus
{
    FileMissing,
    NoChanges,
    VersionCreated
}

public sealed class SaveSyncResult
{
    public required SaveSyncStatus Status { get; init; }

    public string? CurrentHash { get; init; }

    public SaveVersion? Version { get; init; }
}
