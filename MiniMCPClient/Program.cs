using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using DotNetEnv;

Env.Load();


var endpoint = new Uri(Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_ENDPOINT"));
var deploymentName = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME");
var apiKey = Environment.GetEnvironmentVariable("API_KEY");

// Create an IChatClient using Azure OpenAI.
IChatClient client =
    new ChatClientBuilder(
        new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey))
        .GetChatClient("gpt-4o").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

// Create the MCP client
// Configure it to start and connect to your MCP server.
IMcpClient mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new()
    {
        Command = "dotnet run",
        Arguments = ["--project", "C:\\develop\\open-source\\azure-ai-agents-dotnet\\MiniMCPServer"],
        Name = "Minimal MCP Server",
    }));

// List all available tools from the MCP server.
Console.WriteLine("Available tools:");
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
foreach (McpClientTool tool in tools)
{
    Console.WriteLine($"{tool}");
}
Console.WriteLine();

// Conversational loop that can utilize the tools via prompts.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Prompt: ");
    messages.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (ChatResponseUpdate update in client
        .GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}
