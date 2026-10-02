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

    private void AddManuallyButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window = new AddGameManuallyWindow
        {
            Owner = this
        };

        if (window.ShowDialog() == true &&
            window.GameAdded)
        {
            DialogResult = true;
        }
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}