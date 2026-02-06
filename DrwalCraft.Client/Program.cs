using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace DrwalCraft.Client;

public static class Program
{
    private record Message(string From, string Text);
    const int Port = 5000;
    public static async Task Main(string[] args)
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
        using var client = new TcpClient();
        await client.ConnectAsync(ipEndPoint);
        Console.WriteLine("Client connected");
        
        var reader = new StreamReader(client.GetStream());
        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

        while (Console.ReadLine() is { } line)
        {
            await writer.WriteLineAsync(line);
            var response = await reader.ReadLineAsync();
             if (CheckMessage(response) == "JSON")
             {
                 Message? msg = JsonSerializer.Deserialize<Message>(response);
                 if (msg != null)
                 {
                     Console.WriteLine(msg.Text + " from:  " + msg.From);
                     if(msg.From != "Serwer") Console.WriteLine("A ty?");
                 }
             }
             else
             {
                 Console.WriteLine(response);
             }
             if (response is null) break;
            //Console.WriteLine(response);
        }
    }
    static string CheckMessage(string msg)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<object>(msg);
            return "JSON";
        }
        catch (JsonException)
        {
            return "String";
        }
    }
}