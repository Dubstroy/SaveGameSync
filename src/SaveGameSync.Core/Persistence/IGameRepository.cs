using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Persistence;

public interface IGameRepository
{
    Task<Game?> GetGameAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Game>> GetAllGamesAsync(
        CancellationToken cancellationToken = default);

    Task AddGameAsync(
        Game game,
        CancellationToken cancellationToken = default);

    Task UpdateGameAsync(
        Game game,
        CancellationToken cancellationToken = default);
}
