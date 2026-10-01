namespace SaveGameSync.Core.Models;

public sealed class TrackedSave
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; init; }

    public required string FilePath { get; init; }

    public bool Exists { get; set; }

    public string? CurrentHash { get; set; }

    public DateTimeOffset? LastDetectedAt { get; set; }
}