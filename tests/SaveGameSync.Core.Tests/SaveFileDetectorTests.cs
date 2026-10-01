using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests;

public class SaveFileDetectorTests
{
    [Fact]
    public void Exists_ReturnsFalse_WhenFileDoesNotExist()
    {
        var detector = new SaveFileDetector();

        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString(),
            "save.sl2");

        var result = detector.Exists(path);

        Assert.False(result);
    }

    [Fact]
    public void Exists_ReturnsTrue_WhenFileExists()
    {
        var detector = new SaveFileDetector();

        var directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, "save.sl2");

        try
        {
            File.WriteAllText(path, "save data");

            var result = detector.Exists(path);

            Assert.True(result);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task CalculateHashAsync_ReturnsHash_WhenFileExists()
    {
        var detector = new SaveFileDetector();

        var directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, "save.sl2");

        try
        {
            File.WriteAllText(path, "save data");

            var hash = await detector.CalculateHashAsync(path);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task CalculateHashAsync_ReturnsNull_WhenFileDoesNotExist()
    {
        var detector = new SaveFileDetector();

        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString(),
            "save.sl2");

        var hash = await detector.CalculateHashAsync(path);

        Assert.Null(hash);
    }
}