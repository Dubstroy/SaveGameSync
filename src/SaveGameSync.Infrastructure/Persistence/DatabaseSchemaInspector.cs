using Microsoft.Data.Sqlite;

namespace SaveGameSync.Infrastructure.Persistence;

public static class DatabaseSchemaInspector
{
    public static void Inspect(string databasePath)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath}");

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
            ORDER BY name;
            """;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
}
