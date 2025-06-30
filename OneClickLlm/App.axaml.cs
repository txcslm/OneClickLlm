using Avalonia;
using Avalonia.Controls;
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

  public override async void OnFrameworkInitializationCompleted()
  {
    // Configure dependency injection services
    Console.WriteLine("Initializing application...");
    AppServices.Configure();
    if (AppServices.Services is null)
      return;

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      Console.WriteLine("Creating main window");
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

      mainPresenter.OpenSettingsRequested += () =>
      {
        var settingsPresenter = AppServices.Services.GetRequiredService<SettingsPresenter>();
        var settingsView = new SettingsView { DataContext = settingsPresenter };

        var dialog = new Window
        {
          Title = "Настройки",
          Content = settingsView,
          Width = 400,
          Height = 300,
          WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        settingsPresenter.CloseRequested += () => dialog.Close();
        dialog.ShowDialog(mainWindow);
      };

      var startPresenter = AppServices.Services.GetRequiredService<ModelSelectionPresenter>();
      var startView = new ModelSelectionView { DataContext = startPresenter };
      var startWindow = new Window
      {
        Title = "Выбор модели",
        Content = startView,
        Width = 600,
        Height = 450,
        WindowStartupLocation = WindowStartupLocation.CenterScreen
      };

      startPresenter.CloseRequested += result =>
      {
        if (result == true)
        {
          mainPresenter.UpdateModelStatus();
          desktop.MainWindow = mainWindow;
          mainWindow.Show();
        }
        else
        {
          desktop.Shutdown();
        }

        startWindow.Close();
      };

      Console.WriteLine("Loading models for start window");
      await startPresenter.LoadModelsAsync();

      desktop.MainWindow = startWindow;
      Console.WriteLine("Initialization completed");
    }

    base.OnFrameworkInitializationCompleted();
  }
}