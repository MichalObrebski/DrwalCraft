using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Messages;
using DrwalCraft.Core;

namespace DrwalCraft.Client;

internal class Client
{
    const int Port = 5000;
    public TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
    
    
    public async Task ConnectAsync()
    {
        Task<bool> isOtherClientConnected = tcs.Task;
        var ipEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
        using var client = new TcpClient();
        await client.ConnectAsync(ipEndPoint);
        Console.WriteLine("Im connected");
        Task read = ReadFromServer(client);
        bool temp = isOtherClientConnected.Result;
        Task write = WriteToServer(client);
        await Task.WhenAll(read, write);
    }

    public async Task ReadFromServer(TcpClient client)
    {
        var reader = new StreamReader(client.GetStream());
        
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null) break;
            Console.WriteLine(line);
            Message? msg = JsonSerializer.Deserialize<Message>(line); 
            if (msg == null) break; 
            if(msg.Text == "Start")
                tcs.SetResult(true);
            else
            {
                lock (ObjectsActions.InQueueLock) 
                { 
                    ObjectsActions.InQueue.Enqueue(msg, msg.Tick); 
                    //ExistingObjects.InSemaphore.Release();
                } 
            }
        }
    }

    public async Task WriteToServer(TcpClient client)
    {
        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        while (true)
        {
            await ObjectsActions.OutSemaphore.WaitAsync();
            Message msg;
            lock (ObjectsActions.OutQueueLock)
            {
                // Console.WriteLine(ExistingObjects.OutQueue.Count);
                msg = ObjectsActions.OutQueue.Dequeue();
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