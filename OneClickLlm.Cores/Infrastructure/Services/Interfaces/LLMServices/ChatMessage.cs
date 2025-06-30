namespace OneClickLlm.Cores.Infrastructure.Services;

/// <summary>
///   Представляет одно сообщение в истории диалога.
/// </summary>
public record ChatMessage(
  ChatMessageRole Role,
  string Content);