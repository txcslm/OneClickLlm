namespace OneClickLlm.Cores.Infrastructure.Services;

/// <summary>
/// Представляет состояние прогресса загрузки файла.
/// </summary>
public record DownloadProgress(
  long? TotalBytes,
  long BytesDownloaded
);