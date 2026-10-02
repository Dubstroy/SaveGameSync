using System.IO;
using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;

namespace SaveGameSync.Core.Services;

public sealed class GameService
{
    private readonly IGameRepository _repository;
    private readonly ISaveRepository _saveRepository;

    public GameService(
        IGameRepository repository,
        ISaveRepository saveRepository)
    {
        _repository = repository;
        _saveRepository = saveRepository;
    }

    public async Task<Game> CreateGameAsync(
    string name,
    string origin,
    string saveFolderPath,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Game name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(origin))
        {
            throw new ArgumentException(
                "Game origin cannot be empty.",
                nameof(origin));
        }

        if (string.IsNullOrWhiteSpace(saveFolderPath))
            throw new ArgumentException(
                "Save folder path cannot be empty.",
                nameof(saveFolderPath));

        var game = new Game
        {
            Name = name.Trim(),
            Origin = origin.Trim(),
            SaveFolderPath = saveFolderPath.Trim()
        };

        await _repository.AddGameAsync(
            game,
            cancellationToken);

        return game;
    }

    public async Task<TrackedSave> AddTrackedSaveAsync(
    Guid gameId,
    string name,
    string filePath,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Save name cannot be empty.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                "Save path cannot be empty.",
                nameof(filePath));

        var game = await _repository.GetGameAsync(
            gameId,
            cancellationToken);

        if (game is null)
            throw new InvalidOperationException(
                $"Game '{gameId}' was not found.");

        var trackedSave = new TrackedSave
        {
            GameId = gameId,
            Name = name.Trim(),
            FilePath = filePath.Trim(),
            Exists = File.Exists(filePath)
        };

        await _saveRepository.AddTrackedSaveAsync(
            trackedSave,
            cancellationToken);

        return trackedSave;
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
            Name = name.Trim(),
            Origin = game.Origin,
            SaveFolderPath = game.SaveFolderPath
        };

        await _repository.UpdateGameAsync(
            updatedGame,
            cancellationToken);
    }
}
