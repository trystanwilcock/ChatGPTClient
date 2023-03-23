using ConsoleGPT;

var chatGPTClient = new ChatGPTClient();
var chatActive = true;

while (chatActive)
{
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage))
    {
        chatActive = false;
    }
    else
    {
        var chatResponse = await chatGPTClient.SendMessage(userMessage);

        if (chatResponse != null)
        {
            Console.WriteLine();

            foreach (var assistantMessage in chatResponse.Choices!.Select(c => c.Message))
                Console.WriteLine(assistantMessage!.Content!.Trim().Replace("\n", ""));

            Console.WriteLine();
        }
        else
        {
            chatActive = false;
        }
    }
}