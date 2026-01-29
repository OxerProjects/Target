using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Target.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // שימוש במודל המהיר והיציב ביותר ל-JSON
        private const string MODEL_NAME = "gemini-2.5-flash";
        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models";

        public AiService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            string url = $"{BASE_URL}/{MODEL_NAME}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    temperature = 0.5,
                    responseMimeType = "application/json"
                }
            };

            try
            {
                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);

                    // חילוץ הטקסט בזהירות
                    if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        return candidates[0]
                           .GetProperty("content")
                           .GetProperty("parts")[0]
                           .GetProperty("text")
                           .GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }
}