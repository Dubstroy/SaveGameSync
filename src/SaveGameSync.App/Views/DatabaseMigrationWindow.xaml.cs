using System.Windows;

namespace SaveGameSync.App.Views;

public partial class DatabaseMigrationWindow : Window
{
    public bool MigrationRequested { get; private set; }

    public bool CreateBackup { get; private set; }

    public DatabaseMigrationWindow()
    {
        InitializeComponent();
    }

    private void BackupRadioButton_Checked(
        object sender,
        RoutedEventArgs e)
    {
        CreateBackup = true;

        MigrateButton.IsEnabled = true;
        WarningTextBlock.Text =
            "Se creará una copia de seguridad antes de realizar la migración.";
    }

    private void NoBackupRadioButton_Checked(
        object sender,
        RoutedEventArgs e)
    {
        CreateBackup = false;

        MigrateButton.IsEnabled = true;
        WarningTextBlock.Text =
            "⚠ La migración se realizará sin crear una copia de seguridad.";
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MigrationRequested = false;
        DialogResult = false;
    }

    private void MigrateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MigrationRequested = true;
        DialogResult = true;
    }
}