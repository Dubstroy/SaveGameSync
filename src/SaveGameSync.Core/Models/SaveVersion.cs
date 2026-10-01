namespace SaveGameSync.Core.Models;

public sealed class SaveVersion
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid TrackedSaveId { get; init; }

    public int VersionNumber { get; init; }

    public required string FileHash { get; init; }

    public required string BackupPath { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
