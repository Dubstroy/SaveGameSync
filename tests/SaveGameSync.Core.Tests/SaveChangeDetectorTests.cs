using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests;

public class SaveChangeDetectorTests
{
    [Fact]
    public void HasChanged_ReturnsFalse_WhenHashesAreEqual()
    {
        var detector = new SaveChangeDetector();

        var result = detector.HasChanged(
            "ABC123",
            "ABC123");

        Assert.False(result);
    }

    [Fact]
    public void HasChanged_ReturnsFalse_WhenHashesDifferOnlyByCase()
    {
        var detector = new SaveChangeDetector();

        var result = detector.HasChanged(
            "ABC123",
            "abc123");

        Assert.False(result);
    }

    [Fact]
    public void HasChanged_ReturnsTrue_WhenHashesAreDifferent()
    {
        var detector = new SaveChangeDetector();

        var result = detector.HasChanged(
            "ABC123",
            "DEF456");

        Assert.True(result);
    }

    [Fact]
    public void HasChanged_ReturnsTrue_WhenThereIsNoPreviousHash()
    {
        var detector = new SaveChangeDetector();

        var result = detector.HasChanged(
            null,
            "ABC123");

        Assert.True(result);
    }

    [Fact]
    public void HasChanged_ReturnsFalse_WhenCurrentHashIsMissing()
    {
        var detector = new SaveChangeDetector();

        var result = detector.HasChanged(
            "ABC123",
            null);

        Assert.False(result);
    }
}
