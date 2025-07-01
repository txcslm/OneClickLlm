using System.Security.Cryptography;
using System.Text.Json;

namespace OneClickLlm.Core.Services;

/// <summary>
/// Provides encrypted storage for chat histories on the local machine.
/// </summary>
public class ChatHistoryService
{
    private readonly string _storagePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OneClickLlm", "Chats");

    private static readonly byte[] _encryptionKey = Convert.FromBase64String("K5v9wUf2gUn8RitYwG8ebw==");
    private static readonly byte[] _encryptionIv = Convert.FromBase64String("9rJk7KG0uE4w2Z1ZEC2X2A==");

    public ChatHistoryService() => Directory.CreateDirectory(_storagePath);

    public async Task SaveAsync(string conversationId, IEnumerable<ChatMessage> messages)
    {
        var filePath = Path.Combine(_storagePath, $"{conversationId}.log");
        var json = JsonSerializer.Serialize(messages);
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _encryptionIv;
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await using var crypto = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await using var writer = new StreamWriter(crypto);
        await writer.WriteAsync(json);
    }
}
