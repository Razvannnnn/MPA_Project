using System.Net.Http.Json;

public class OllamaService
{
    private readonly HttpClient _httpClient;

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> AskAsync(string prompt)
    {
        var request = new
        {
            model = "qwen3.5:4b",
            prompt,
            think = false,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
        return result?.Response?.Trim() ?? string.Empty;
    }

    private sealed class OllamaResponse
    {
        public string? Response { get; set; }
    }
}