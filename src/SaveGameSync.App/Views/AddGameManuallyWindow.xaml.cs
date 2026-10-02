using System.Windows;

namespace SaveGameSync.App.Views;

public partial class AddGameManuallyWindow : Window
{
    public AddGameManuallyWindow()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Seleccionar carpeta del guardado"
        };

        if (dialog.ShowDialog() == true)
        {
            SavePathTextBox.Text = dialog.FolderName;
        }
    }

    private void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MessageBox.Show(
            "La creación del juego se implementará en el siguiente paso.",
            "SaveGameSync",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
