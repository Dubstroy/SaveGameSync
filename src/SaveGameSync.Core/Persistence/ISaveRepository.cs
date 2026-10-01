using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Persistence;

public interface ISaveRepository
{
    Task<TrackedSave?> GetTrackedSaveAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SaveVersion>> GetVersionsAsync(
        Guid trackedSaveId,
        CancellationToken cancellationToken = default);

    Task AddTrackedSaveAsync(
        TrackedSave trackedSave,
        CancellationToken cancellationToken = default);

    Task UpdateTrackedSaveAsync(
        TrackedSave trackedSave,
        CancellationToken cancellationToken = default);

    Task AddVersionAsync(
        SaveVersion version,
        CancellationToken cancellationToken = default);
}
