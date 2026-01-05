using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Target.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // שים לב: השם המדויק מה-JSON ששלחת
        private const string MODEL_NAME = "gemini-2.5-flash";
        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1/models";

        public AiService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            // בניית ה-URL עם המודל הנכון
            string url = $"{BASE_URL}/{MODEL_NAME}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                // הוספת thinking=true אם תרצה לנצל את יכולות ה"מחשבה" של מודל 2.5
                generationConfig = new {
                    temperature = 0.4,    // מוריד יצירתיות ואת הצורך בחשיבה
                }

            };

            try
            {
                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseString);
                    return doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString()?.Trim();
                }

                return $"API Error: {response.StatusCode}. Details: {responseString}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}