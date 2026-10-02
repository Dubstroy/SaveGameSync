namespace SaveGameSync.Core.Models;

public sealed class Game
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; init; }

    public required string Origin { get; init; }

    public required string SaveFolderPath { get; init; }

    public List<TrackedSave> TrackedSaves { get; init; } = [];

}
