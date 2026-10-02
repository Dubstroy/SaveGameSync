using System.Windows;

namespace SaveGameSync.App.Views;

public partial class AddGameWindow : Window
{
    public AddGameWindow()
    {
        InitializeComponent();
    }

    private void SearchButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MessageBox.Show(
            "La búsqueda de bibliotecas se implementará más adelante.",
            "SaveGameSync",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private async void AddManuallyButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window = new AddGameManuallyWindow
        {
            Owner = this
        };

        if (window.ShowDialog() != true ||
            !window.GameAdded)
        {
            return;
        }

        try
        {
            await ((App)Application.Current)
                .Services
                .GameService
                .CreateGameWithTrackedSavesAsync(
                    window.GameName,
                    window.GameOrigin,
                    window.SaveFolderPath,
                    window.SelectedFiles);

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo guardar el juego.\n\n{ex.Message}",
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