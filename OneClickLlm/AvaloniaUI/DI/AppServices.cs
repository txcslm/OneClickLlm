using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using OneClickLlm.AvaloniaUI.Presenters;
using OneClickLlm.AvaloniaUI.Views;
using OneClickLlm.Core.Services;
using OneClickLlm.Cores.Infrastructure.Services;

namespace OneClickLlm.AvaloniaUI;

public static class AppServices
{
  public static IServiceProvider? Services { get; private set; }

  public static void Configure()
  {
    var services = new ServiceCollection();

    // Регистрация сервисов (Core)
    services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost:11434") });
    services.AddSingleton<IModelManager, LocalModelManager>();
    services.AddSingleton<ILlmService, OllamaLlmService>();
        
    // Регистрация Presenter'ов
    services.AddSingleton<MainWindowPresenter>();
    services.AddTransient<ModelSelectionPresenter>(); // Transient, т.к. создается при каждом открытии окна
        
    // Регистрация Views (Окон/Контролов)
    services.AddTransient<MainWindow>();
    services.AddTransient<ModelSelectionView>();

    Services = services.BuildServiceProvider();
  }
}