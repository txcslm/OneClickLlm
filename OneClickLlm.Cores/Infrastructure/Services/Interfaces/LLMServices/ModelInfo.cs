namespace OneClickLlm.Cores.Infrastructure.Services;

/// <summary>
///   Представляет метаданные языковой модели.
/// </summary>
public record ModelInfo(
  string Id,
  string DisplayName,
  float SizeGb,
  string Description,
  bool IsLocal);