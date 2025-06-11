using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Tell a bad joke")]
    public static async Task<string> Joke()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://api.chucknorris.io/jokes/random");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var joke = doc.RootElement.GetProperty("value").GetString();
        return joke ?? "Chuck Norris joke not found.";
    }

    [McpServerTool, Description("Tell a good joke")]
    public static string GoodJoke()
    {
        return "Why did the scarecrow win an award? Because he was outstanding in his field!";
    }

}