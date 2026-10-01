using Microsoft.EntityFrameworkCore;

namespace SaveGameSync.Infrastructure.Persistence;

public static class SaveGameDatabaseFactory
{
    public static SaveGameDbContext Create(string databasePath)
    {
        var directory = Path.GetDirectoryName(databasePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new DbContextOptionsBuilder<SaveGameDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        return new SaveGameDbContext(options);
    }
}
