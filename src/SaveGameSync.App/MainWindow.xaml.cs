using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SaveGameSync.Core.Models;
using SaveGameSync.App.Views;

namespace SaveGameSync.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await LoadGamesAsync();
    }

    private async void AddGameButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window = new AddGameWindow
        {
            Owner = this
        };

        window.ShowDialog();

        await LoadGamesAsync();
    }

    private async Task LoadGamesAsync()
    {
        var games = await ((App)Application.Current)
            .Services
            .GameService
            .GetAllGamesAsync();

        GamesPanel.Children.Clear();

        foreach (var game in games)
        {
            GamesPanel.Children.Add(
                CreateGameCard(game));
        }
    }

    private Button CreateGameCard(Game game)
    {
        var button = new Button
        {
            Width = 180,
            Height = 250,
            Margin = new Thickness(12),
            Padding = new Thickness(0),
            Tag = game
        };

        var panel = new StackPanel();

        var cover = new Border
        {
            Height = 190,
            Background = Brushes.LightGray,
            CornerRadius = new CornerRadius(8)
        };

        var coverText = new TextBlock
        {
            Text = "Sin carátula",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.Gray,
            FontSize = 16
        };

        cover.Child = coverText;

        var name = new TextBlock
        {
            Text = game.Name,
            FontWeight = FontWeights.SemiBold,
            FontSize = 16,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(8)
        };

        panel.Children.Add(cover);
        panel.Children.Add(name);

        button.Content = panel;

        button.Click += GameCard_Click;

        return button;
    }

    private async void GameCard_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not Game game)
        {
            return;
        }

        try
        {
            var selectedGame =
                await ((App)Application.Current)
                    .Services
                    .GameService
                    .GetGameAsync(game.Id);

            if (selectedGame is null)
            {
                MessageBox.Show(
                    "No se pudo encontrar el juego.",
                    "SaveGameSync",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            SelectedGameNameTextBlock.Text =
                selectedGame.Name;

            SelectedGameOriginTextBlock.Text =
                $"Origen: {selectedGame.Origin}";

            SelectedGamePathTextBlock.Text =
                $"Carpeta del guardado: {selectedGame.SaveFolderPath}";

            LoadTrackedSaves(selectedGame);

            LibraryView.Visibility =
                Visibility.Collapsed;

            GameView.Visibility =
                Visibility.Visible;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo cargar el juego.\n\n{ex.Message}",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void LoadTrackedSaves(Game game)
    {
        TrackedSavesPanel.Children.Clear();

        foreach (var save in game.TrackedSaves)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(4, 6, 4, 6)
            };

            var name = new TextBlock
            {
                Text = save.Name,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold
            };

            var path = new TextBlock
            {
                Text = save.FilePath,
                Foreground = Brushes.Gray,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12,
                Margin = new Thickness(0, 4, 0, 0)
            };

            var status = new TextBlock
            {
                Text = save.Exists
                    ? "● Archivo encontrado"
                    : "● Archivo no encontrado",
                Foreground = save.Exists
                    ? Brushes.Green
                    : Brushes.Red,
                FontSize = 12,
                Margin = new Thickness(0, 4, 0, 0)
            };

            panel.Children.Add(name);
            panel.Children.Add(path);
            panel.Children.Add(status);

            TrackedSavesPanel.Children.Add(panel);
        }
    }

    private void BackToLibraryButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        GameView.Visibility =
            Visibility.Collapsed;

        LibraryView.Visibility =
            Visibility.Visible;
    }
}