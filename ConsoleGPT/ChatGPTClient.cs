using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ConsoleGPT
{
    public class ChatGPTClient
    {
        private readonly string chatRequestUri;
        private readonly string openAIAPIKey;
        private readonly List<Message> messageHistory;
        private readonly bool includeHistoryWithChatCompletion;
        private readonly OpenAIModels model;
        private readonly double temperature;
        private readonly double top_p;

        public ChatGPTClient(bool includeHistoryWithChatCompletion = true,
            OpenAIModels model = OpenAIModels.gpt_3_5_turbo,
            double temperature = 1,
            double top_p = 1)
        {
            chatRequestUri = "https://api.openai.com/v1/chat/completions";
            openAIAPIKey = Environment.GetEnvironmentVariable("OpenAIAPIKey")!;
            messageHistory = new List<Message>();
            this.includeHistoryWithChatCompletion = includeHistoryWithChatCompletion;
            this.model = model;
            this.temperature = temperature;
            this.top_p = top_p;
        }

        public async Task<ChatResponse?> SendMessage(string message)
        {
            var chatResponse = await Chat(message);

            if (chatResponse != null)
            {
                AddMessageToHistory(new Message { Role = "user", Content = message });

                foreach (var responseMessage in chatResponse.Choices!.Select(c => c.Message!))
                    AddMessageToHistory(responseMessage);
            }

            return chatResponse;
        }

        private async Task<ChatResponse?> Chat(string message)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, chatRequestUri);
            request.Headers.Add("Authorization", $"Bearer {openAIAPIKey}");

            var requestBody = new
            {
                model = GetModel(),
                temperature,
                top_p,
                messages = GetMessageObjects(message)
            };

            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>();

                if (chatResponse != null &&
                    chatResponse.Choices != null &&
                    chatResponse.Choices.Any(c => c.Message != null))
                    return chatResponse;
            }

            return null;
        }

        private IEnumerable<object> GetMessageObjects(string message)
        {
            foreach (var historicMessage in includeHistoryWithChatCompletion ? messageHistory : Enumerable.Empty<Message>())
            {
                yield return new { role = historicMessage.Role, content = historicMessage.Content };
            }

            yield return new { role = "user", content = message };
        }

        private void AddMessageToHistory(Message message) =>
            messageHistory.Add(message);

        private string GetModel() =>
            model.ToString().Replace("3_5", "3.5").Replace("_", "-");

    }

    public class ChatResponse
    {
        public string? Id { get; set; }
        public string? Object { get; set; }
        public int Created { get; set; }
        public List<Choice>? Choices { get; set; }
        public Usage? Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message? Message { get; set; }
        public string? Finish_Reason { get; set; }
    }

    public class Message
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
    }

    public class Usage
    {
        public int Prompt_Tokens { get; set; }
        public int Completion_Tokens { get; set; }
        public int Total_Tokens { get; set; }
    }

    public enum OpenAIModels
    {
        gpt_4,
        gpt_4_0314,
        gpt_4_32k,
        gpt_4_32k_0314,
        gpt_3_5_turbo,
        gpt_3_5_turbo_0301
    }
}