using Microsoft.Extensions.DependencyInjection;
using System;
using OneClickLlm.AvaloniaUI.Presenters;
using OneClickLlm.AvaloniaUI.Views;
using OneClickLlm.AvaloniaUI.Services;
using OneClickLlm.Core.Services;

namespace OneClickLlm.AvaloniaUI;

public static class AppServices
{
  public static IServiceProvider? Services { get; private set; }

  public static void Configure()
  {
    var services = new ServiceCollection();

    // Core services
    services.AddSingleton<ILlmService, LocalLlamaSharpService>();
    services.AddSingleton<ChatLogService>();
    services.AddSingleton<IFilePickerService, FilePickerService>();
        
    // Регистрация Presenter'ов
    services.AddSingleton<MainWindowPresenter>();
    services.AddTransient<ModelSelectionPresenter>(); // Transient, т.к. создается при каждом открытии окна
        
    // Регистрация Views (Окон/Контролов)
    services.AddTransient<MainWindow>();
    services.AddTransient<ModelSelectionView>();

    Services = services.BuildServiceProvider();
  }
}