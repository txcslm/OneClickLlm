using System.Security.Cryptography;
using System.Text.Json;

namespace OneClickLlm.Core.Services;

/// <summary>
/// Provides encrypted storage for chat histories on the local machine.
/// </summary>
public class ChatLogService
{
    private readonly string _logDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OneClickLlm", "Chats");

    private static readonly byte[] _key = Convert.FromBase64String("K5v9wUf2gUn8RitYwG8ebw==");
    private static readonly byte[] _iv = Convert.FromBase64String("9rJk7KG0uE4w2Z1ZEC2X2A==");

    public ChatLogService() => Directory.CreateDirectory(_logDirectory);

    public async Task SaveAsync(string conversationId, IEnumerable<ChatMessage> messages)
    {
        var filePath = Path.Combine(_logDirectory, $"{conversationId}.log");
        var json = JsonSerializer.Serialize(messages);
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await using var crypto = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await using var writer = new StreamWriter(crypto);
        await writer.WriteAsync(json);
    }
}
