# ChatGPTClient

ChatGPTClient is a C# class that can be used to generate responses from the ChatGPT API and retain conversation context.

The repository also includes a console application demonstrating its use.

ChatGPTClient was written for the following blog post:

https://trystanwilcock.com/2023/03/23/how-to-use-chatgpt-in-c-sharp-and-writing-a-chatgpt-client/

Configuration:

Set an Environment variable for "OpenAIAPIKey".

Usage:

```var chatGPTClient = new ChatGPTClient();```

```var chatResponse = await chatGPTClient.SendMessage(userMessage);```
