using System.IO;
using System.Windows;
using SaveGameSync.App.Services;
using SaveGameSync.App.Views;

namespace SaveGameSync.App;

public partial class App : Application
{
    public AppServices Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var databasePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "SaveGameSync",
            "SaveGameSync.db");

        try
        {
            var databaseExists = File.Exists(databasePath);

            Services = await AppServices.CreateAsync(databasePath);

            if (!databaseExists)
            {
                await Services.Database.MigrateAsync();

                Services.Dispose();

                Services =
                    await AppServices.CreateAsync(
                        databasePath);
            }
            else if (Services.PendingMigrations.Count > 0)
            {
                var migrationWindow =
                    new DatabaseMigrationWindow();

                var result =
                    migrationWindow.ShowDialog();

                if (result != true)
                {
                    Shutdown();
                    return;
                }

                if (migrationWindow.CreateBackup)
                {
                    var backupFileName =
                        $"SaveGameSync_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";

                    var backupPath =
                        Path.Combine(
                            Path.GetDirectoryName(databasePath)!,
                            backupFileName);

                    await Services.Database.BackupAsync(
                        backupPath);
                }

                await Services.Database.MigrateAsync();

                Services.Dispose();

                Services =
                    await AppServices.CreateAsync(
                        databasePath);
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();

            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo iniciar SaveGameSync.\n\n{ex.Message}",
                "Error al iniciar",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Services?.Dispose();
        base.OnExit(e);
    }
}