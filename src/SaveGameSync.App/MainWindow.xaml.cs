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

    private void GameCard_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not Game game)
        {
            return;
        }

        MessageBox.Show(
            $"Seleccionaste: {game.Name}",
            "SaveGameSync",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}