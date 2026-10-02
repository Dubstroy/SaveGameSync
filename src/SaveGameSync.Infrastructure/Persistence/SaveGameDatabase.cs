using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SaveGameDatabase
{
    private readonly SaveGameDbContext _dbContext;
    private readonly string _databasePath;

    public SaveGameDatabase(
        SaveGameDbContext dbContext,
        string databasePath)
    {
        _dbContext = dbContext;
        _databasePath = databasePath;
    }

    public async Task<IReadOnlyList<string>> GetPendingMigrationsAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken))
            .ToList();
    }

    public async Task MigrateAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(
            cancellationToken);
    }

    public async Task BackupAsync(
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        var sourceConnectionString =
            $"Data Source={_databasePath}";

        var destinationConnectionString =
            $"Data Source={backupPath}";

        await using var sourceConnection =
            new SqliteConnection(sourceConnectionString);

        await using var destinationConnection =
            new SqliteConnection(destinationConnectionString);

        await sourceConnection.OpenAsync(cancellationToken);
        await destinationConnection.OpenAsync(cancellationToken);

        sourceConnection.BackupDatabase(destinationConnection);
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await MigrateAsync(cancellationToken);
    }
}