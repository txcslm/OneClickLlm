using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneClickLlm.Core.Services
{
    /// <summary>
    /// Управляет языковыми моделями, взаимодействуя с локальным API Ollama.
    /// Реализует интерфейс <see cref="IModelManager"/>.
    /// </summary>
    public class LocalModelManager : IModelManager
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public LocalModelManager(HttpClient httpClient) => _httpClient = httpClient;

        #region DTOs for Ollama API
        private class OllamaTagsResponse { [JsonPropertyName("models")] public OllamaModelDetail[] Models { get; set; } = Array.Empty<OllamaModelDetail>(); }
        private class OllamaModelDetail { [JsonPropertyName("name")] public string Name { get; set; } = string.Empty; [JsonPropertyName("size")] public long Size { get; set; } [JsonPropertyName("modified_at")] public DateTime ModifiedAt { get; set; } }
        private class OllamaPullRequest { [JsonPropertyName("name")] public string Name { get; set; } = string.Empty; }
        private class OllamaPullStatus { [JsonPropertyName("status")] public string Status { get; set; } = string.Empty; [JsonPropertyName("total")] public long? Total { get; set; } [JsonPropertyName("completed")] public long? Completed { get; set; } }
        private class OllamaDeleteRequest { [JsonPropertyName("name")] public string Name { get; set; } = string.Empty; }
        #endregion

        public async Task<IEnumerable<ModelInfo>> GetLocalModelsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<OllamaTagsResponse>("/api/tags", _jsonSerializerOptions, cancellationToken);
                return response?.Models.Select(m => new ModelInfo(
                    Id: m.Name, DisplayName: m.Name,
                    SizeGb: (float)Math.Round(m.Size / 1_073_741_824.0, 2),
                    Description: $"Modified: {m.ModifiedAt:yyyy-MM-dd HH:mm}", IsLocal: true
                )) ?? Enumerable.Empty<ModelInfo>();
            }
            catch (HttpRequestException e)
            {
                throw new InvalidOperationException("Не удалось подключиться к API Ollama. Убедитесь, что сервис запущен.", e);
            }
        }

        public async Task DeleteModelAsync(string modelId, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/delete")
            {
                Content = new StringContent(JsonSerializer.Serialize(new OllamaDeleteRequest { Name = modelId }), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public Task DownloadModelAsync(ModelInfo model, IProgress<DownloadProgress> progress, CancellationToken cancellationToken = default)
        {
            // В MVP не реализуется.
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ModelInfo>> GetRemoteModelsAsync(string filter, CancellationToken cancellationToken = default)
        {
            // В MVP не реализуется.
            return Task.FromResult(Enumerable.Empty<ModelInfo>());
        }
    }
}