using Microsoft.EntityFrameworkCore;
using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SqliteGameRepository : IGameRepository
{
    private readonly SaveGameDbContext _dbContext;

    public SqliteGameRepository(SaveGameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Game?> GetGameAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .Include(game => game.TrackedSaves)
            .SingleOrDefaultAsync(
                game => game.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Game>> GetAllGamesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .Include(game => game.TrackedSaves)
            .OrderBy(game => game.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddGameAsync(
        Game game,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Games.Add(game);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateGameAsync(
        Game game,
        CancellationToken cancellationToken = default)
    {
        var existingGame = await _dbContext.Games
            .SingleOrDefaultAsync(
                existing => existing.Id == game.Id,
                cancellationToken);

        if (existingGame is null)
        {
            throw new InvalidOperationException(
                $"Game '{game.Id}' was not found.");
        }

        _dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(game);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
