using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;
using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests.Services;

public sealed class GameServiceTests
{
    [Fact]
    public async Task CreateGameAsync_ShouldCreateAndPersistGame()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var game = await service.CreateGameAsync(
            "Dark Souls III",
            "Steam");

        Assert.NotEqual(
            Guid.Empty,
            game.Id);

        Assert.Equal(
            "Dark Souls III",
            game.Name);

        var savedGame = await repository.GetGameAsync(
            game.Id);

        Assert.NotNull(savedGame);

        Assert.Equal(
            "Dark Souls III",
            savedGame.Name);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldTrimGameName()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var game = await service.CreateGameAsync(
            "  Dark Souls III  ",
            "Steam");

        Assert.Equal(
            "Dark Souls III",
            game.Name);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldRejectEmptyName()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameAsync("   ", "Steam"));
    }

    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnAllGames()
    {
        var repository = new InMemoryGameRepository();

        var firstGame = new Game
        {
            Name = "Dark Souls III",
            Origin = "Steam"
        };

        var secondGame = new Game
        {
            Name = "Dark Souls Remastered",
            Origin = "Steam"
        };

        await repository.AddGameAsync(firstGame);
        await repository.AddGameAsync(secondGame);

        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var games = await service.GetAllGamesAsync();

        Assert.Equal(
            2,
            games.Count);

        Assert.Contains(
            games,
            game => game.Name == "Dark Souls III");

        Assert.Contains(
            games,
            game => game.Name == "Dark Souls Remastered");
    }

    [Fact]
    public async Task GetGameAsync_ShouldReturnGame()
    {
        var repository = new InMemoryGameRepository();

        var game = new Game
        {
            Name = "Dark Souls III",
            Origin = "Steam"
        };

        await repository.AddGameAsync(game);

        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var result = await service.GetGameAsync(
            game.Id);

        Assert.NotNull(result);

        Assert.Equal(
            game.Id,
            result.Id);

        Assert.Equal(
            "Dark Souls III",
            result.Name);
    }

    [Fact]
    public async Task GetGameAsync_ShouldReturnNullForUnknownId()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var result = await service.GetGameAsync(
            Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateGameNameAsync_ShouldUpdateName()
    {
        var repository = new InMemoryGameRepository();

        var game = new Game
        {
            Name = "Dark Souls",
            Origin = "Steam"
        };

        await repository.AddGameAsync(game);

        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        await service.UpdateGameNameAsync(
            game.Id,
            "Dark Souls III");

        var updatedGame = await repository.GetGameAsync(
            game.Id);

        Assert.NotNull(updatedGame);

        Assert.Equal(
            "Dark Souls III",
            updatedGame.Name);
    }

    [Fact]
    public async Task UpdateGameNameAsync_ShouldRejectEmptyName()
    {
        var repository = new InMemoryGameRepository();

        var game = new Game
        {
            Name = "Dark Souls III",
            Origin = "Steam"
        };

        await repository.AddGameAsync(game);

        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateGameNameAsync(
                game.Id,
                "   "));
    }


    [Fact]
    public async Task UpdateGameNameAsync_ShouldRejectUnknownGame()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateGameNameAsync(
                Guid.NewGuid(),
                "Dark Souls III"));
    }

    [Fact]
    public async Task CreateGameAsync_ShouldPersistOrigin()
    {
        var repository = new InMemoryGameRepository();
        var saveRepository = new InMemorySaveRepository();

        var service = new GameService(
            repository,
            saveRepository);

        var game = await service.CreateGameAsync(
            "Dark Souls III",
            "Steam");

        var savedGame = await repository.GetGameAsync(game.Id);

        Assert.NotNull(savedGame);
        Assert.Equal("Steam", savedGame.Origin);
    }

    private sealed class InMemorySaveRepository : ISaveRepository
    {
        private readonly List<TrackedSave> _trackedSaves = [];
        private readonly List<SaveVersion> _versions = [];

        public Task<TrackedSave?> GetTrackedSaveAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var trackedSave = _trackedSaves
                .SingleOrDefault(save => save.Id == id);

            return Task.FromResult(trackedSave);
        }

        public Task<IReadOnlyList<SaveVersion>> GetVersionsAsync(
            Guid trackedSaveId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<SaveVersion> versions = _versions
                .Where(version => version.TrackedSaveId == trackedSaveId)
                .OrderBy(version => version.VersionNumber)
                .ToList();

            return Task.FromResult(versions);
        }

        public Task AddTrackedSaveAsync(
            TrackedSave trackedSave,
            CancellationToken cancellationToken = default)
        {
            _trackedSaves.Add(trackedSave);

            return Task.CompletedTask;
        }

        public Task UpdateTrackedSaveAsync(
            TrackedSave trackedSave,
            CancellationToken cancellationToken = default)
        {
            var existingSave = _trackedSaves
                .SingleOrDefault(
                    existing => existing.Id == trackedSave.Id);

            if (existingSave is null)
            {
                throw new InvalidOperationException(
                    $"Tracked save '{trackedSave.Id}' was not found.");
            }

            var index = _trackedSaves.IndexOf(existingSave);

            _trackedSaves[index] = trackedSave;

            return Task.CompletedTask;
        }

        public Task AddVersionAsync(
            SaveVersion version,
            CancellationToken cancellationToken = default)
        {
            _versions.Add(version);

            return Task.CompletedTask;
        }
    }
    private sealed class InMemoryGameRepository : IGameRepository
    {
        private readonly List<Game> _games = [];

        public Task<Game?> GetGameAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var game = _games
                .SingleOrDefault(game => game.Id == id);

            return Task.FromResult(game);
        }

        public Task<IReadOnlyList<Game>> GetAllGamesAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Game> games = _games
                .OrderBy(game => game.Name)
                .ToList();

            return Task.FromResult(games);
        }

        public Task AddGameAsync(
            Game game,
            CancellationToken cancellationToken = default)
        {
            _games.Add(game);

            return Task.CompletedTask;
        }

        public Task UpdateGameAsync(
            Game game,
            CancellationToken cancellationToken = default)
        {
            var existingGame = _games
                .SingleOrDefault(
                    existing => existing.Id == game.Id);

            if (existingGame is null)
            {
                throw new InvalidOperationException(
                    $"Game '{game.Id}' was not found.");
            }

            var index = _games.IndexOf(existingGame);

            _games[index] = game;

            return Task.CompletedTask;
        }
    }
}
