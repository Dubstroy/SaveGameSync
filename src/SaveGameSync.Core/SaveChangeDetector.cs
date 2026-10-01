namespace SaveGameSync.Core.Services;

public sealed class SaveChangeDetector
{
    public bool HasChanged(string? previousHash, string? currentHash)
    {
        if (string.IsNullOrWhiteSpace(currentHash))
            return false;

        if (string.IsNullOrWhiteSpace(previousHash))
            return true;

        return !string.Equals(
            previousHash,
            currentHash,
            StringComparison.OrdinalIgnoreCase);
    }
}