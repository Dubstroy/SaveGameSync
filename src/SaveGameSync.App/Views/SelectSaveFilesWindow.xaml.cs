using System.Windows;
using System.Windows.Controls;
using SaveGameSync.Core.Models;

namespace SaveGameSync.App.Views;

public partial class SelectSaveFilesWindow : Window
{
    private readonly List<SaveFileCandidate> _files;

    public IReadOnlyList<SaveFileCandidate> SelectedFiles { get; private set; }
        = [];

    public SelectSaveFilesWindow(
        IReadOnlyList<SaveFileCandidate> files)
    {
        InitializeComponent();

        _files = files.ToList();

        LoadFiles();
    }

    private void LoadFiles()
    {
        FilesPanel.Children.Clear();

        foreach (var file in _files)
        {
            var checkBox = new CheckBox
            {
                Content = CreateFileContent(file),
                IsChecked = file.IsSelected,
                Margin = new Thickness(4, 6, 4, 6),
                Tag = file
            };

            checkBox.Checked += FileCheckBox_Checked;
            checkBox.Unchecked += FileCheckBox_Unchecked;

            FilesPanel.Children.Add(checkBox);
        }
    }

    private static FrameworkElement CreateFileContent(
        SaveFileCandidate file)
    {
        var panel = new StackPanel();

        var name = new TextBlock
        {
            Text = file.Name,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold
        };

        var details = new TextBlock
        {
            Text = FormatFileSize(file.Size),
            Foreground = System.Windows.Media.Brushes.Gray,
            FontSize = 12
        };

        panel.Children.Add(name);
        panel.Children.Add(details);

        return panel;
    }

    private void FileCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox &&
            checkBox.Tag is SaveFileCandidate file)
        {
            file.IsSelected = true;
        }
    }

    private void FileCheckBox_Unchecked(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox &&
            checkBox.Tag is SaveFileCandidate file)
        {
            file.IsSelected = false;
        }
    }

    private void SelectAllButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        foreach (var file in _files)
        {
            file.IsSelected = true;
        }

        LoadFiles();
    }

    private void SelectNoneButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        foreach (var file in _files)
        {
            file.IsSelected = false;
        }

        LoadFiles();
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        SelectedFiles = _files
            .Where(file => file.IsSelected)
            .ToList();

        if (SelectedFiles.Count == 0)
        {
            MessageBox.Show(
                "Selecciona al menos un archivo para continuar.",
                "SaveGameSync",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        DialogResult = true;
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static string FormatFileSize(long size)
    {
        if (size < 1024)
            return $"{size} B";

        if (size < 1024 * 1024)
            return $"{size / 1024.0:F1} KB";

        if (size < 1024 * 1024 * 1024)
            return $"{size / (1024.0 * 1024.0):F1} MB";

        return $"{size / (1024.0 * 1024.0 * 1024.0):F1} GB";
    }
}
