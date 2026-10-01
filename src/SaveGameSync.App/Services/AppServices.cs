using SaveGameSync.Core.Services;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.App.Services;

public sealed class AppServices : IDisposable
{
    private readonly SaveGameDbContext _dbContext;

    public GameService GameService { get; }

    private AppServices(
        SaveGameDbContext dbContext,
        GameService gameService)
    {
        _dbContext = dbContext;
        GameService = gameService;
    }

    public static async Task<AppServices> CreateAsync(
        string databasePath,
        CancellationToken cancellationToken = default)
    {
        var dbContext =
            SaveGameDatabaseFactory.Create(databasePath);

        var database =
            new SaveGameDatabase(dbContext);

        await database.InitializeAsync();

        var gameRepository =
            new SqliteGameRepository(dbContext);

        var gameService =
            new GameService(gameRepository);

        return new AppServices(
            dbContext,
            gameService);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}