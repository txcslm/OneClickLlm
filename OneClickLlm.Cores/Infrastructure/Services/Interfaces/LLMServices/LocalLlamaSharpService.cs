using System.Runtime.CompilerServices;
using LLama;
using LLama.Common;

namespace OneClickLlm.Core.Services;

/// <summary>
/// Provides a local LLM service using LLamaSharp library.
/// </summary>
public class LocalLlamaSharpService : ILlmService
{
    private LLamaWeights? _weights;
    private LLamaContext? _context;
    private ChatSession? _session;

    public ModelInfo? CurrentModel { get; private set; }

    public Task LoadModelAsync(string modelPath, CancellationToken cancellationToken = default)
    {
        _weights = LLamaWeights.LoadFromFile(modelPath);
        _context = _weights.CreateContext(new LLamaContextParams());
        _session = new ChatSession(new InteractiveExecutor(_context));
        CurrentModel = new ModelInfo(modelPath, Path.GetFileName(modelPath), 0, "Local GGUF model", true);
        return Task.CompletedTask;
    }

    public Task UnloadModelAsync(CancellationToken cancellationToken = default)
    {
        _session = null;
        _context?.Dispose();
        _context = null;
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

        foreach (var msg in history)
        {
            if (msg.Role == ChatMessageRole.User)
                _session.AddUserMessage(msg.Content);
            else
                _session.AddAssistantMessage(msg.Content);
        }

        await foreach (var token in _session.StreamAsync(prompt, cancellationToken))
        {
            yield return token;
        }
    }
}
