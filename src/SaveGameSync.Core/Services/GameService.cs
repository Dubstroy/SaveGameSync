using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;

namespace SaveGameSync.Core.Services;

public sealed class GameService
{
    private readonly IGameRepository _repository;

    public GameService(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<Game> CreateGameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Game name cannot be empty.",
                nameof(name));
        }

        var game = new Game
        {
            Name = name.Trim()
        };

        await _repository.AddGameAsync(
            game,
            cancellationToken);

        return game;
    }

    public async Task<IReadOnlyList<Game>> GetAllGamesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllGamesAsync(
            cancellationToken);
    }

    public async Task<Game?> GetGameAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetGameAsync(
            id,
            cancellationToken);
    }

    public async Task UpdateGameNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Game name cannot be empty.",
                nameof(name));
        }

        var game = await _repository.GetGameAsync(
            id,
            cancellationToken);

        if (game is null)
        {
            throw new InvalidOperationException(
                $"Game '{id}' was not found.");
        }

        var updatedGame = new Game
        {
            Id = game.Id,
            Name = name.Trim()
        };

        await _repository.UpdateGameAsync(
            updatedGame,
            cancellationToken);
    }
}
