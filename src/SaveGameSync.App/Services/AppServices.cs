using SaveGameSync.Core.Services;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.App.Services;

public sealed class AppServices : IDisposable
{
    private readonly SaveGameDbContext _dbContext;

    public SaveGameDatabase Database { get; }

    public GameService GameService { get; }

    public IReadOnlyList<string> PendingMigrations { get; }

    private AppServices(
        SaveGameDbContext dbContext,
        SaveGameDatabase database,
        GameService gameService,
        IReadOnlyList<string> pendingMigrations)
    {
        _dbContext = dbContext;
        Database = database;
        GameService = gameService;
        PendingMigrations = pendingMigrations;
    }

    public static async Task<AppServices> CreateAsync(
        string databasePath,
        CancellationToken cancellationToken = default)
    {
        var dbContext =
            SaveGameDatabaseFactory.Create(databasePath);

        var database =
            new SaveGameDatabase(
                dbContext,
                databasePath);

        var pendingMigrations =
            await database.GetPendingMigrationsAsync(
                cancellationToken);

        var gameRepository =
            new SqliteGameRepository(dbContext);

        var saveRepository =
            new SqliteSaveRepository(dbContext);

        var gameService =
            new GameService(
                gameRepository,
                saveRepository);

        return new AppServices(
            dbContext,
            database,
            gameService,
            pendingMigrations);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}