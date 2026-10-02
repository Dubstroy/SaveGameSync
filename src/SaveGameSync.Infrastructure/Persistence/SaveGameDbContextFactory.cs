using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SaveGameDbContextFactory
    : IDesignTimeDbContextFactory<SaveGameDbContext>
{
    public SaveGameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SaveGameDbContext>();

        optionsBuilder.UseSqlite(
            "Data Source=:memory:");

        return new SaveGameDbContext(optionsBuilder.Options);
    }
}
