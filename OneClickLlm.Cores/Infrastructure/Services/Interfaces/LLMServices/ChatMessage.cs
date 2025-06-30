namespace OneClickLlm.Core.Services;

/// <summary>
///   Представляет одно сообщение в истории диалога.
/// </summary>
public record ChatMessage(
  ChatMessageRole Role,
  string Content);