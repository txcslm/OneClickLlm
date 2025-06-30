using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OneClickLlm.AvaloniaUI.Views;
using Microsoft.Extensions.DependencyInjection;
using OneClickLlm.AvaloniaUI.Presenters;
using OneClickLlm.AvaloniaUI;

namespace OneClickLlm;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    // Configure dependency injection services
    AppServices.Configure();
    if (AppServices.Services is null)
      return;

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      var mainPresenter = AppServices.Services.GetRequiredService<MainWindowPresenter>();
      var mainWindow = AppServices.Services.GetRequiredService<MainWindow>();

      mainWindow.DataContext = mainPresenter;

      mainPresenter.OpenModelSelectionRequested += async () =>
      {
        var modelSelectionPresenter = AppServices.Services.GetRequiredService<ModelSelectionPresenter>();
        var modelSelectionView = new ModelSelectionView { DataContext = modelSelectionPresenter };

        var dialog = new Window
        {
          Title = "Выбор модели",
          Content = modelSelectionView,
          Width = 600,
          Height = 450,
          WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        modelSelectionPresenter.CloseRequested += result => dialog.Close(result);
        await modelSelectionPresenter.LoadModelsAsync();

        var result = await dialog.ShowDialog<bool?>(mainWindow);

        if (result == true)
        {
          mainPresenter.UpdateModelStatus();
        }
      };

      desktop.MainWindow = mainWindow;
    }

    base.OnFrameworkInitializationCompleted();
  }
}