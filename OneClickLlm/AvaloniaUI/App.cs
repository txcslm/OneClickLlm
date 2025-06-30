using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OneClickLlm.AvaloniaUI.Presenters;
using OneClickLlm.AvaloniaUI.Views;

namespace OneClickLlm.AvaloniaUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Конфигурируем все зависимости
        AppServices.Configure();
        if (AppServices.Services is null) return;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 2. Получаем Presenter и View главного окна из DI
            var mainPresenter = AppServices.Services.GetRequiredService<MainWindowPresenter>();
            var mainWindow = AppServices.Services.GetRequiredService<MainWindow>();

            // 3. Связываем View и Presenter
            mainWindow.DataContext = mainPresenter;
            
            // 4. Подписываемся на события от Presenter'а для открытия дочерних окон
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

                modelSelectionPresenter.CloseRequested += (result) => dialog.Close(result);
                await modelSelectionPresenter.LoadModelsAsync();
                
                var result = await dialog.ShowDialog<bool?>(mainWindow);

                if (result == true)
                {
                    mainPresenter.UpdateModelStatus();
                }
            };
            
            // 5. Назначаем главное окно
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}