using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OneClickLlm.AvaloniaUI.Views;

public partial class ModelSelectionView : UserControl
{
  public ModelSelectionView()
  {
    InitializeComponent();
  }

  private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is Presenters.ModelSelectionPresenter presenter && VisualRoot is Window window)
    {
      await presenter.BrowseForModelAsync(window);
    }
  }
}