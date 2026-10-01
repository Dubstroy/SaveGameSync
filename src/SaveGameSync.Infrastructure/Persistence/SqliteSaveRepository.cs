using Microsoft.EntityFrameworkCore;
using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SqliteSaveRepository : ISaveRepository
{
    private readonly SaveGameDbContext _dbContext;

    public SqliteSaveRepository(SaveGameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrackedSave?> GetTrackedSaveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.TrackedSaves
            .SingleOrDefaultAsync(
                save => save.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<SaveVersion>> GetVersionsAsync(
        Guid trackedSaveId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveVersions
            .Where(version =>
                version.TrackedSaveId == trackedSaveId)
            .OrderBy(version => version.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddTrackedSaveAsync(
        TrackedSave trackedSave,
        CancellationToken cancellationToken = default)
    {
        _dbContext.TrackedSaves.Add(trackedSave);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateTrackedSaveAsync(
        TrackedSave trackedSave,
        CancellationToken cancellationToken = default)
    {
        _dbContext.TrackedSaves.Update(trackedSave);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AddVersionAsync(
        SaveVersion version,
        CancellationToken cancellationToken = default)
    {
        _dbContext.SaveVersions.Add(version);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
