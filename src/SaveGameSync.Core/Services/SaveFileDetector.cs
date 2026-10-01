using System.Security.Cryptography;

namespace SaveGameSync.Core.Services;

public sealed class SaveFileDetector
{
    public bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    public async Task<string?> CalculateHashAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return null;

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        using var sha256 = SHA256.Create();

        var hash = await sha256.ComputeHashAsync(
            stream,
            cancellationToken);

        return Convert.ToHexString(hash);
    }
}