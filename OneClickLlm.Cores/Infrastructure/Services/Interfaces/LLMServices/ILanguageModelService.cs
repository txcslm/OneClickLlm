namespace OneClickLlm.Core.Services;

/// <summary>
///   Определяет контракт для сервиса, непосредственно взаимодействующего с языковой моделью.
/// </summary>
public interface ILanguageModelService
{
  /// <summary>
  ///   Получает информацию о модели, которая в данный момент загружена и готова к использованию.
  /// </summary>
  ModelInfo? CurrentModel { get; }

  /// <summary>
  ///   Асинхронно загружает указанную модель в память.
  /// </summary>
  Task LoadModelAsync(string modelId, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Асинхронно выгружает текущую модель из памяти.
  /// </summary>
  Task UnloadModelAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Асинхронно генерирует ответ на заданный промпт в потоковом режиме.
  /// </summary>
  IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt,
    IEnumerable<ChatMessage> history,
    GenerationOptions options,
    CancellationToken cancellationToken = default);
}