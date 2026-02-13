using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Messages;
using DrwalCraft.Core;

namespace DrwalCraft.Client;

public class Client
{
    const int Port = 5000;
    
    
    public async Task ConnectAsync()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
        using var client = new TcpClient();
        await client.ConnectAsync(ipEndPoint);
        Console.WriteLine("Client connected");
        Task read = ReadFromServer(client);
        Task write = WriteToServer(client);
        await Task.WhenAll(read, write);
    }

    public async Task ReadFromServer(TcpClient client)
    {
        var reader = new StreamReader(client.GetStream());
        int i = 0;
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null) break;
            Message? msg = JsonSerializer.Deserialize<Message>(line); 
            if (msg == null) break; 
            lock (ExistingObjects.InQueueLock) 
            { 
                ExistingObjects.InQueue.Enqueue(msg, msg.Tick); 
                //ExistingObjects.InSemaphore.Release();
            } 
        }
    }

    public async Task WriteToServer(TcpClient client)
    {
        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        while (true)
        {
            await ExistingObjects.OutSemaphore.WaitAsync();
            Message msg;
            lock (ExistingObjects.OutQueueLock)
            {
                // Console.WriteLine(ExistingObjects.OutQueue.Count);
                msg = ExistingObjects.OutQueue.Dequeue();
                // Console.WriteLine(ExistingObjects.OutQueue.Count);
                
            }
            string json = JsonSerializer.Serialize(msg);
            await writer.WriteLineAsync(json);
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