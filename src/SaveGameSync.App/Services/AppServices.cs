using SaveGameSync.Core.Services;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.App.Services;

public sealed class AppServices : IDisposable
{
    private readonly SaveGameDbContext _dbContext;

    public SaveGameDatabase Database { get; }

    public GameService GameService { get; }

    public SaveSyncService SaveSyncService { get; }

    public IReadOnlyList<string> PendingMigrations { get; }

    private AppServices(
    SaveGameDbContext dbContext,
    SaveGameDatabase database,
    GameService gameService,
    SaveSyncService saveSyncService,
    IReadOnlyList<string> pendingMigrations)
    {
        _dbContext = dbContext;
        Database = database;
        GameService = gameService;
        SaveSyncService = saveSyncService;
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

        var fileDetector =
    new SaveFileDetector();

        var changeDetector =
            new SaveChangeDetector();

        var versionManager =
            new SaveVersionManager();

        var backupService =
            new SaveBackupService();

        var saveSyncService =
            new SaveSyncService(
                fileDetector,
                changeDetector,
                versionManager,
                backupService,
                saveRepository);

        return new AppServices(
            dbContext,
            database,
            gameService,
            saveSyncService,
            pendingMigrations);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}