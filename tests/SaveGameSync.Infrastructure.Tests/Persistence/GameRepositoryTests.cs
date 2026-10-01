using Microsoft.Data.Sqlite;
using SaveGameSync.Core.Models;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.Infrastructure.Tests.Persistence;

public sealed class GameRepositoryTests
{
    [Fact]
    public async Task AddGameAsync_ShouldPersistGame()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository = new SqliteGameRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            await repository.AddGameAsync(game);

            var savedGame =
                await repository.GetGameAsync(game.Id);

            Assert.NotNull(savedGame);

            Assert.Equal(
                "Dark Souls III",
                savedGame.Name);

            Assert.Equal(
                game.Id,
                savedGame.Id);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnGamesOrderedByName()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository = new SqliteGameRepository(db);

            await repository.AddGameAsync(new Game
            {
                Name = "The Witcher 3"
            });

            await repository.AddGameAsync(new Game
            {
                Name = "Dark Souls III"
            });

            await repository.AddGameAsync(new Game
            {
                Name = "Elden Ring"
            });

            var games =
                await repository.GetAllGamesAsync();

            Assert.Equal(3, games.Count);

            Assert.Equal(
                "Dark Souls III",
                games[0].Name);

            Assert.Equal(
                "Elden Ring",
                games[1].Name);

            Assert.Equal(
                "The Witcher 3",
                games[2].Name);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldPersistChanges()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository = new SqliteGameRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            await repository.AddGameAsync(game);

            var updatedGame = new Game
            {
                Id = game.Id,
                Name = "Dark Souls III - Updated"
            };

            await repository.UpdateGameAsync(updatedGame);

            var savedGame =
                await repository.GetGameAsync(game.Id);

            Assert.NotNull(savedGame);

            Assert.Equal(
                "Dark Souls III - Updated",
                savedGame.Name);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "SaveGameSyncTests",
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        return directory;
    }
}
