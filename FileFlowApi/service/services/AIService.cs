using System.Text;
using System.Text.Json;
using core.IServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using core.Models;
using System.Net.Http.Json;

public class AIService : IAIService
{
    //private readonly HttpClient _httpClient;
    //private readonly string _apiKey;
    //private readonly IConfiguration _configuration;


    //public AiService(IConfiguration configuration)
    //{
    //    _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini API key is missing from configuration");

    //    _httpClient = new HttpClient();
    //    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

    //}

    //public async Task<string> ClassifyDocumentAsync(string fileContent, List<string> userCategories)
    //{
    //    var prompt = BuildPrompt(fileContent, userCategories);

    //    var requestBody = new
    //    {
    //        contents = new[]
    //        {
    //            new
    //            {
    //                parts = new[]
    //                {
    //                    new
    //                    {
    //                        text = prompt
    //                    }
    //                }
    //            }
    //        }
    //    };

    //    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

    //    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

    //    var response = await _httpClient.PostAsync(url, content);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var responseString = await response.Content.ReadAsStringAsync();
    //        var result = JsonDocument.Parse(responseString);

    //        var text = result.RootElement
    //            .GetProperty("candidates")[0]
    //            .GetProperty("content")
    //            .GetProperty("parts")[0]
    //            .GetProperty("text")
    //            .GetString();

    //        return text?.Trim() ?? "אין התאמה";
    //    }

    //    return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
    //}
    //public AIService(IConfiguration configuration)
    //{
    //    _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini API key is missing from configuration");

    //    _httpClient = new HttpClient();
    //    // אין להוסיף Authorization Header כאן
    //    _configuration = configuration;
    //}

    //public async Task<string> ClassifyDocumentAsync(string fileContent, List<string> userCategories)
    //{
    //    var prompt = BuildPrompt(fileContent, userCategories);

    //    var requestBody = new
    //    {
    //        contents = new[]
    //        {
    //        new
    //        {
    //            parts = new[]
    //            {
    //                new
    //                {
    //                    text = prompt
    //                }
    //            }
    //        }
    //    }
    //    };

    //    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiKey}";

    //    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

    //    var response = await _httpClient.PostAsync(url, content);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var responseString = await response.Content.ReadAsStringAsync();
    //        var result = JsonDocument.Parse(responseString);

    //        var text = result.RootElement
    //            .GetProperty("candidates")[0]
    //            .GetProperty("content")
    //            .GetProperty("parts")[0]
    //            .GetProperty("text")
    //            .GetString();

    //        return text?.Trim() ?? "אין התאמה";
    //    }

    //    return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
    //}

    //private string BuildPrompt(string fileContent, List<string> categories)
    //{
    //    var categoriesList = string.Join("\n- ", categories);
    //    return $"למשתמש קיימות הקטגוריות הבאות:\n- {categoriesList}\n\n" +
    //           $"המסמך הבא הועלה עם תוכן או שם הקובץ:\n\"{fileContent}\"\n\n" +
    //           "בחר אך ורק אחת מהקטגוריות למעלה שאליה שייך המסמך. אם אין התאמה מתאימה, החזר בדיוק את המילים 'אין התאמה'.\n" +
    //           "תשובה:";
    //}

    //public async Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories)
    //{
    //    var apiKey = _configuration["OpenAI:ApiKey"];
    //    var prompt = new
    //    {
    //        model = "gpt-4o-mini",
    //        messages = new[]
    //        {
    //            new { role = "system", content = "אתה מערכת לסיווג מסמכים לפי קטגוריות שניתנו." },
    //            new {
    //                role = "user",
    //                content = $"המסמך הבא:\n{fileContent}\n\nהקטגוריות האפשריות הן:\n{string.Join(", ", userCategories)}\n\nאיזו קטגוריה הכי מתאימה למסמך הזה? ענה בשם הקטגוריה בלבד."
    //            }
    //        }
    //    };

    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
    //    {
    //        Content = JsonContent.Create(prompt)
    //    };
    //    request.Headers.Add("Authorization", $"Bearer {apiKey}");

    //    var response = await _httpClient.SendAsync(request);
    //    response.EnsureSuccessStatusCode();

    //    var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
    //    return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
    //}


   
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public async Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories)
        {
        //var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var apiKey = _configuration["OPENAI_API_KEY"];
        Console.WriteLine("API KEY !!!!!!!!!!!!!!!!!!= " + _configuration["OPENAI_API_KEY"]);


        Console.WriteLine($"Loaded API Key: {apiKey}");

        var prompt = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new { role = "system", content = "אתה מערכת לסיווג מסמכים לפי קטגוריות שניתנו." },
                new {
                    role = "user",
                    content = $"המסמך הבא:\n{fileContent}\n\nהקטגוריות האפשריות הן:\n{string.Join(", ", userCategories)}\n\nאיזו קטגוריה הכי מתאימה למסמך הזה? ענה בשם הקטגוריה בלבד."
                }
            }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(prompt)
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
        }
   

}
