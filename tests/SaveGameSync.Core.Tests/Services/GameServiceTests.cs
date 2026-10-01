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
        var service = new GameService(repository);

        var game = await service.CreateGameAsync(
            "Dark Souls III");

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
        var service = new GameService(repository);

        var game = await service.CreateGameAsync(
            "  Dark Souls III  ");

        Assert.Equal(
            "Dark Souls III",
            game.Name);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldRejectEmptyName()
    {
        var repository = new InMemoryGameRepository();
        var service = new GameService(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameAsync("   "));
    }

    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnAllGames()
    {
        var repository = new InMemoryGameRepository();

        var firstGame = new Game
        {
            Name = "Dark Souls III"
        };

        var secondGame = new Game
        {
            Name = "Dark Souls Remastered"
        };

        await repository.AddGameAsync(firstGame);
        await repository.AddGameAsync(secondGame);

        var service = new GameService(repository);

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
            Name = "Dark Souls III"
        };

        await repository.AddGameAsync(game);

        var service = new GameService(repository);

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
        var service = new GameService(repository);

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
            Name = "Dark Souls"
        };

        await repository.AddGameAsync(game);

        var service = new GameService(repository);

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
            Name = "Dark Souls III"
        };

        await repository.AddGameAsync(game);

        var service = new GameService(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateGameNameAsync(
                game.Id,
                "   "));
    }

    [Fact]
    public async Task UpdateGameNameAsync_ShouldRejectUnknownGame()
    {
        var repository = new InMemoryGameRepository();
        var service = new GameService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateGameNameAsync(
                Guid.NewGuid(),
                "Dark Souls III"));
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
