using System.IO;
using System.Windows;
using SaveGameSync.App.Services;

namespace SaveGameSync.App;

public partial class App : Application
{
    public AppServices Services { get; private set; } = null!;

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        var databasePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "SaveGameSync",
            "SaveGameSync.db");

        Services = await AppServices.CreateAsync(
            databasePath);
    }

    protected override void OnExit(
        ExitEventArgs e)
    {
        Services.Dispose();

        base.OnExit(e);
    }
}
