using System.Runtime.CompilerServices;
using LLama;
using LLama.Common;

namespace OneClickLlm.Core.Services;

/// <summary>
/// Provides a local LLM service using LLamaSharp library.
/// </summary>
public class LocalLanguageModelService : ILanguageModelService
{
    private LLamaWeights? _weights;
    private LLamaContext? _context;
    private ChatSession? _session;

    public ModelInfo? CurrentModel { get; private set; }

    public Task LoadModelAsync(string modelPath, CancellationToken cancellationToken = default)
    {
        var modelParams = new ModelParams(modelPath);
        _weights = LLamaWeights.LoadFromFile(modelParams);
        _context = _weights.CreateContext(modelParams);
        _session = new ChatSession(new InteractiveExecutor(_context));
        CurrentModel = new ModelInfo(modelPath, Path.GetFileName(modelPath), 0, "Local GGUF model", true);
        return Task.CompletedTask;
    }

    public Task UnloadModelAsync(CancellationToken cancellationToken = default)
    {
        _session = null;
        _context?.Dispose();
        _context = null;
        _weights?.Dispose();
        _weights = null;
        CurrentModel = null;
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt,
        IEnumerable<ChatMessage> history, GenerationOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_session == null)
            throw new InvalidOperationException("Model not loaded");

        var chatHistory = new ChatHistory();
        foreach (var msg in history)
        {
            chatHistory.AddMessage(msg.Role == ChatMessageRole.User ? AuthorRole.User : AuthorRole.Assistant, msg.Content);
        }
        chatHistory.AddMessage(AuthorRole.User, prompt);

        var infer = new InferenceParams
        {
           
        };

        await foreach (var token in _session.ChatAsync(chatHistory, infer, cancellationToken))
        {
            yield return token;
        }
    }
}
