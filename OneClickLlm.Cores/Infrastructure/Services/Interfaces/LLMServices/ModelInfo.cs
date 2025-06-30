namespace OneClickLlm.Core.Services;

/// <summary>
///   Представляет метаданные языковой модели.
/// </summary>
public record ModelInfo(
  string Id,
  string DisplayName,
  float SizeGb,
  string Description,
  bool IsLocal);