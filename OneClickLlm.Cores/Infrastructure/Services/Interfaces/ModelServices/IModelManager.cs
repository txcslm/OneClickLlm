namespace OneClickLlm.Cores.Infrastructure.Services;

/// <summary>
/// Предоставляет контракт для управления языковыми моделями.
/// Отвечает за получение списка доступных моделей (как локальных, так и удаленных),
/// их загрузку с поддержкой отслеживания прогресса и удаление с диска.
/// </summary>
public interface IModelManager
{
  /// <summary>
  /// Асинхронно получает список моделей, уже загруженных и доступных локально.
  /// </summary>
  Task<IEnumerable<ModelInfo>> GetLocalModelsAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Асинхронно получает список моделей, доступных для загрузки из удаленного репозитория.
  /// </summary>
  Task<IEnumerable<ModelInfo>> GetRemoteModelsAsync(string filter, CancellationToken cancellationToken = default);

  /// <summary>
  /// Асинхронно загружает указанную модель из удаленного репозитория.
  /// </summary>
  Task DownloadModelAsync(ModelInfo model, IProgress<DownloadProgress> progress, CancellationToken cancellationToken = default);

  /// <summary>
  /// Асинхронно удаляет локально сохраненную модель.
  /// </summary>
  Task DeleteModelAsync(string modelId, CancellationToken cancellationToken = default);
}