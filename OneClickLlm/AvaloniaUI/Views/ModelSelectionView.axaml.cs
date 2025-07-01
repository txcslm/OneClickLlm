using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OneClickLlm.AvaloniaUI.Views;

public partial class ModelSelectionView : UserControl
{
    public event Func<Window, Task>? BrowseModelRequested;

    public ModelSelectionView() => InitializeComponent();

    private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window && BrowseModelRequested is not null)
            await BrowseModelRequested.Invoke(window);
    }
}