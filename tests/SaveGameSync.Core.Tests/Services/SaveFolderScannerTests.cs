using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests.Services;

public sealed class SaveFolderScannerTests
{
    [Fact]
    public void Scan_ShouldReturnFilesFromFolder()
    {
        var temporaryDirectory =
            Directory.CreateTempSubdirectory();

        try
        {
            var firstFile =
                Path.Combine(
                    temporaryDirectory.FullName,
                    "save1.sl2");

            var secondFile =
                Path.Combine(
                    temporaryDirectory.FullName,
                    "save2.bak");

            File.WriteAllText(firstFile, "test");
            File.WriteAllText(secondFile, "test");

            var scanner = new SaveFolderScanner();

            var result =
                scanner.Scan(temporaryDirectory.FullName);

            Assert.Equal(2, result.Count);

            Assert.Contains(
                result,
                file => file.Name == "save1.sl2");

            Assert.Contains(
                result,
                file => file.Name == "save2.bak");
        }
        finally
        {
            temporaryDirectory.Delete(
                recursive: true);
        }
    }

    [Fact]
    public void Scan_ShouldReturnFilesOrderedByName()
    {
        var temporaryDirectory =
            Directory.CreateTempSubdirectory();

        try
        {
            File.WriteAllText(
                Path.Combine(
                    temporaryDirectory.FullName,
                    "z-save.dat"),
                "test");

            File.WriteAllText(
                Path.Combine(
                    temporaryDirectory.FullName,
                    "a-save.dat"),
                "test");

            var scanner = new SaveFolderScanner();

            var result =
                scanner.Scan(temporaryDirectory.FullName);

            Assert.Equal(
                "a-save.dat",
                result[0].Name);

            Assert.Equal(
                "z-save.dat",
                result[1].Name);
        }
        finally
        {
            temporaryDirectory.Delete(
                recursive: true);
        }
    }

    [Fact]
    public void Scan_ShouldNotIncludeFilesFromSubdirectories()
    {
        var temporaryDirectory =
            Directory.CreateTempSubdirectory();

        try
        {
            File.WriteAllText(
                Path.Combine(
                    temporaryDirectory.FullName,
                    "root-save.dat"),
                "test");

            var subdirectory =
                Directory.CreateDirectory(
                    Path.Combine(
                        temporaryDirectory.FullName,
                        "SubFolder"));

            File.WriteAllText(
                Path.Combine(
                    subdirectory.FullName,
                    "nested-save.dat"),
                "test");

            var scanner = new SaveFolderScanner();

            var result =
                scanner.Scan(temporaryDirectory.FullName);

            Assert.Single(result);

            Assert.Equal(
                "root-save.dat",
                result[0].Name);
        }
        finally
        {
            temporaryDirectory.Delete(
                recursive: true);
        }
    }

    [Fact]
    public void Scan_ShouldReturnFilesUnselectedByDefault()
    {
        var temporaryDirectory =
            Directory.CreateTempSubdirectory();

        try
        {
            File.WriteAllText(
                Path.Combine(
                    temporaryDirectory.FullName,
                    "save.dat"),
                "test");

            var scanner = new SaveFolderScanner();

            var result =
                scanner.Scan(temporaryDirectory.FullName);

            Assert.Single(result);
            Assert.False(result[0].IsSelected);
        }
        finally
        {
            temporaryDirectory.Delete(
                recursive: true);
        }
    }

    [Fact]
    public void Scan_ShouldRejectMissingFolder()
    {
        var scanner = new SaveFolderScanner();

        var missingFolder =
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());

        Assert.Throws<DirectoryNotFoundException>(
            () => scanner.Scan(missingFolder));
    }
}