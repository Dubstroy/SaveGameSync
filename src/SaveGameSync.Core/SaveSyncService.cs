using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Services;

public sealed class SaveSyncService
{
    private readonly SaveFileDetector _fileDetector;
    private readonly SaveChangeDetector _changeDetector;
    private readonly SaveVersionManager _versionManager;
    private readonly SaveBackupService _backupService;

    public SaveSyncService(
        SaveFileDetector fileDetector,
        SaveChangeDetector changeDetector,
        SaveVersionManager versionManager,
        SaveBackupService backupService)
    {
        _fileDetector = fileDetector;
        _changeDetector = changeDetector;
        _versionManager = versionManager;
        _backupService = backupService;
    }

    public async Task<SaveSyncResult> SynchronizeAsync(
        TrackedSave trackedSave,
        IEnumerable<SaveVersion> existingVersions,
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        if (!_fileDetector.Exists(trackedSave.FilePath))
        {
            return new SaveSyncResult
            {
                Status = SaveSyncStatus.FileMissing
            };
        }

        var currentHash = await _fileDetector.CalculateHashAsync(
            trackedSave.FilePath,
            cancellationToken);

        if (!_changeDetector.HasChanged(
                trackedSave.CurrentHash,
                currentHash))
        {
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

        return new SaveSyncResult
        {
            Status = SaveSyncStatus.VersionCreated,
            CurrentHash = currentHash,
            Version = version
        };
    }
}
