using Microsoft.EntityFrameworkCore;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SaveGameDatabase
{
    private readonly SaveGameDbContext _dbContext;

    public SaveGameDatabase(SaveGameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(
            cancellationToken);
    }
}
