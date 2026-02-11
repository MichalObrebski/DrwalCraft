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
        // Task game = Task.Run(async() => await GameLoop(client));
        Task game = GameLoop(client);
        Task read = ReadFromServer(client);
        Task write = WriteToServer(client);
        await Task.WhenAll(game, read, write);
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
                ExistingObjects.InQueue.Enqueue(msg, i); 
                //ExistingObjects.InSemaphore.Release();
            } 
            i++;
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

    public async Task GameLoop(TcpClient client)
    {
        Task enqueue = GameLoopEnqueue(client); //tutaj są kładzione akcje gracze zarówno na InQueue - akcje do wykonania
        //przez gracza jak i na OutQueue - akcje do wykonania przez innych graczy i serwer
        Task dequeue = GameLoopDequeue(client);
        
        await Task.WhenAll(dequeue, enqueue);
    }
    
    public async Task GameLoopDequeue(TcpClient client)
    {
        while (true)
        {
            await ExistingObjects.InSemaphore.WaitAsync();
            lock (ExistingObjects.InQueueLock)
            {
                if (ExistingObjects.InQueue.Count > 0)
                {
                    var msg  = ExistingObjects.InQueue.Dequeue();
                    if (msg.From == client.Client.LocalEndPoint.ToString()) 
                        Console.WriteLine("Jebiesz komunizm!");
                    else
                    {
                        Console.WriteLine(msg.Text + " from:  " + msg.From);
                        //Serwer zwraca from: RemoteEndPoint będący adresem klienta
                        if(msg.From != "Serwer") Console.WriteLine("A ty?");
                    }
                }
            }
        }
    }

    
    public async Task GameLoopEnqueue(TcpClient client)
    {
        while (true)
        {
            Task<string?> readTask = Task.Run(() => Console.ReadLine());
            string? read = await readTask; //zapewnia przełączenie wątku na wykonanie GameLoopDequeue kiedy gracz nic nie robi
            if(read == null) break;
            Message msg;
            if (read == "jebac komunizm")
            {
                msg = new Message(client.Client.LocalEndPoint.ToString(), "jebac komunizm");
                lock (ExistingObjects.InQueueLock)
                {
                    ExistingObjects.InQueue.Enqueue(msg, 1);
                }
                lock (ExistingObjects.OutQueueLock)
                {
                    ExistingObjects.OutQueue.Enqueue(msg, 1);
                }
                ExistingObjects.OutSemaphore.Release();
                ExistingObjects.InSemaphore.Release();
            }
            else
            {
                msg = new Message(client.Client.LocalEndPoint.ToString(), read);
                lock (ExistingObjects.OutQueueLock)
                {
                    ExistingObjects.OutQueue.Enqueue(msg, 1);
                }
                ExistingObjects.OutSemaphore.Release();
            }
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