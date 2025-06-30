namespace OneClickLlm.Core.Services;

/// <summary>
///   Представляет настраиваемые параметры для процесса генерации текста.
/// </summary>
public record GenerationOptions(
  float? Temperature,
  float? TopP);