using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneClickLlm.Core.Services;

namespace OneClickLlm.Core.Services
{
    /// <summary>
    /// Предоставляет сервис для взаимодействия с LLM через API Ollama.
    /// </summary>
    public class OllamaLanguageModelService : ILanguageModelService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public OllamaLanguageModelService(HttpClient httpClient) => _httpClient = httpClient;
        
        public ModelInfo? CurrentModel { get; private set; }

        #region DTOs for Ollama API
        private class OllamaChatRequest { [JsonPropertyName("model")] public string Model { get; set; } = string.Empty; [JsonPropertyName("messages")] public IEnumerable<OllamaChatMessage> Messages { get; set; } = Enumerable.Empty<OllamaChatMessage>(); [JsonPropertyName("stream")] public bool Stream { get; set; } = true; [JsonPropertyName("options")] public OllamaGenerationOptions? Options { get; set; } }
        private class OllamaChatMessage { [JsonPropertyName("role")] public string Role { get; set; } = string.Empty; [JsonPropertyName("content")] public string Content { get; set; } = string.Empty; }
        private class OllamaGenerationOptions { [JsonPropertyName("temperature")] public float? Temperature { get; set; } [JsonPropertyName("top_p")] public float? TopP { get; set; } }
        private class OllamaChatResponse { [JsonPropertyName("message")] public OllamaChatMessage? Message { get; set; } [JsonPropertyName("done")] public bool Done { get; set; } }
        #endregion
        
        public async Task LoadModelAsync(string modelId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new { name = modelId };
                using var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/show") { Content = content };
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                CurrentModel = new ModelInfo(modelId, modelId, 0, "Description not available", true);
            }
            catch (HttpRequestException e)
            {
                throw new InvalidOperationException($"Модель '{modelId}' не найдена или API Ollama недоступен.", e);
            }
        }

        public Task UnloadModelAsync(CancellationToken cancellationToken = default)
        {
            CurrentModel = null;
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt, IEnumerable<ChatMessage> history, GenerationOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (CurrentModel is null) throw new InvalidOperationException("Ни одна модель не загружена.");
            
            var messages = history.Select(h => new OllamaChatMessage { Role = h.Role.ToString().ToLower(), Content = h.Content }).ToList();
            messages.Add(new OllamaChatMessage { Role = "user", Content = prompt });

            var request = new OllamaChatRequest {
                Model = CurrentModel.Id, Messages = messages, Stream = true,
                Options = new OllamaGenerationOptions { Temperature = options.Temperature, TopP = options.TopP }
            };
            
            using var jsonContent = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, "application/json");
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
            {
                Content = jsonContent
            };

            using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var chatResponse = JsonSerializer.Deserialize<OllamaChatResponse>(line, _jsonSerializerOptions);
                if (chatResponse?.Message?.Content != null) yield return chatResponse.Message.Content;
                if (chatResponse is { Done: true }) break;
            }
        }
    }
}