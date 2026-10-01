namespace SaveGameSync.Core.Models;

public sealed class Game
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; init; }

    public List<TrackedSave> TrackedSaves { get; init; } = [];
}
