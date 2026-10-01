using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;

namespace SaveGameSync.Core.Services;

public sealed class SaveSyncService
{
    private readonly SaveFileDetector _fileDetector;
    private readonly SaveChangeDetector _changeDetector;
    private readonly SaveVersionManager _versionManager;
    private readonly SaveBackupService _backupService;
    private readonly ISaveRepository _repository;

    public SaveSyncService(
        SaveFileDetector fileDetector,
        SaveChangeDetector changeDetector,
        SaveVersionManager versionManager,
        SaveBackupService backupService,
        ISaveRepository repository)
    {
        _fileDetector = fileDetector;
        _changeDetector = changeDetector;
        _versionManager = versionManager;
        _backupService = backupService;
        _repository = repository;
    }

    public async Task<SaveSyncResult> SynchronizeAsync(
        Guid trackedSaveId,
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        var trackedSave = await _repository.GetTrackedSaveAsync(
            trackedSaveId,
            cancellationToken);

        if (trackedSave is null)
        {
            throw new InvalidOperationException(
                $"Tracked save '{trackedSaveId}' was not found.");
        }

        var existingVersions = await _repository.GetVersionsAsync(
            trackedSaveId,
            cancellationToken);

        var detectedAt = DateTimeOffset.UtcNow;

        if (!_fileDetector.Exists(trackedSave.FilePath))
        {
            trackedSave.Exists = false;
            trackedSave.LastDetectedAt = detectedAt;

            await _repository.UpdateTrackedSaveAsync(
                trackedSave,
                cancellationToken);

            return new SaveSyncResult
            {
                Status = SaveSyncStatus.FileMissing
            };
        }

        var currentHash = await _fileDetector.CalculateHashAsync(
            trackedSave.FilePath,
            cancellationToken);

        trackedSave.Exists = true;
        trackedSave.LastDetectedAt = detectedAt;

        if (!_changeDetector.HasChanged(
                trackedSave.CurrentHash,
                currentHash))
        {
            await _repository.UpdateTrackedSaveAsync(
                trackedSave,
                cancellationToken);

            return new SaveSyncResult
            {
                Status = SaveSyncStatus.NoChanges,
                CurrentHash = currentHash
            };
        }

        var version = _versionManager.CreateVersion(
            trackedSave,
            currentHash!,
            backupPath,
            existingVersions);

        await _backupService.CopyAsync(
            trackedSave.FilePath,
            backupPath,
            cancellationToken);

        trackedSave.CurrentHash = currentHash;

        await _repository.UpdateTrackedSaveAsync(
            trackedSave,
            cancellationToken);

        await _repository.AddVersionAsync(
            version,
            cancellationToken);

        return new SaveSyncResult
        {
            Status = SaveSyncStatus.VersionCreated,
            CurrentHash = currentHash,
            Version = version
        };
    }
}