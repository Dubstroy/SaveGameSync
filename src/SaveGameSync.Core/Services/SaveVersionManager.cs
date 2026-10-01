using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Services;

public sealed class SaveVersionManager
{
    public SaveVersion CreateVersion(
        TrackedSave trackedSave,
        string fileHash,
        string backupPath,
        IEnumerable<SaveVersion> existingVersions)
    {
        var nextVersionNumber = existingVersions
            .Where(version => version.TrackedSaveId == trackedSave.Id)
            .Select(version => version.VersionNumber)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return new SaveVersion
        {
            TrackedSaveId = trackedSave.Id,
            VersionNumber = nextVersionNumber,
            FileHash = fileHash,
            BackupPath = backupPath
        };
    }
}
