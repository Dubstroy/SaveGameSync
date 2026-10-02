using SaveGameSync.Core.Services;
using System.Windows;
using System.Windows.Controls;

namespace SaveGameSync.App.Views;

public partial class AddGameManuallyWindow : Window
{
    public AddGameManuallyWindow()
    {
        InitializeComponent();
    }

    public bool GameAdded { get; private set; }

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

    private void ContinueButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(GameNameTextBox.Text))
        {
            MessageBox.Show(
                "Introduce el nombre del juego.",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (OriginComboBox.SelectedItem is not ComboBoxItem selectedOrigin)
        {
            MessageBox.Show(
                "Selecciona el origen del juego.",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (string.IsNullOrWhiteSpace(SavePathTextBox.Text))
        {
            MessageBox.Show(
                "Selecciona la carpeta del guardado.",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            var scanner = new SaveFolderScanner();

            var files = scanner.Scan(
                SavePathTextBox.Text);

            if (files.Count == 0)
            {
                MessageBox.Show(
                    "No se encontraron archivos en la carpeta seleccionada.",
                    "SaveGameSync",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var selectionWindow =
                new SelectSaveFilesWindow(files)
                {
                    Owner = this
                };

            if (selectionWindow.ShowDialog() != true)
                return;

            var selectedFiles =
                selectionWindow.SelectedFiles;

            MessageBox.Show(
                $"Se seleccionaron {selectedFiles.Count} archivo(s).",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            GameAdded = true;
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo analizar la carpeta.\n\n{ex.Message}",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
