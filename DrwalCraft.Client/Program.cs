using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace DrwalCraft.Client;

public static class Program
{
    private record Message(string From, string Text);
    const int Port = 5000;
    private static readonly PriorityQueue<Message,int> InQueue = new PriorityQueue<Message, int>();
    private static readonly PriorityQueue<string,int> OutQueue = new PriorityQueue<string, int>();
    private static readonly Lock InQueueLock = new Lock();
    private static readonly Lock OutQueueLock = new Lock();
    private static readonly SemaphoreSlim OutSemaphore = new SemaphoreSlim(0);
    private static readonly SemaphoreSlim InSemaphore = new SemaphoreSlim(0);
    
    
    public static async Task Main(string[] args)
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

    public static async Task ReadFromServer(TcpClient client)
    {
        var reader = new StreamReader(client.GetStream());
        int i = 0;
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null) break;
            if (CheckMessage(line) == "JSON")
            { 
                Message? msg = JsonSerializer.Deserialize<Message>(line);
                if (msg == null) break;
                lock (InQueueLock)
                {
                    InQueue.Enqueue(msg, i);
                    InSemaphore.Release();
                }
                i++;
            }
        }
    }

    public static async Task WriteToServer(TcpClient client)
    {
        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        while (true)
        {
            await OutSemaphore.WaitAsync();
            string msg;
            lock (OutQueueLock)
            {
                msg = OutQueue.Dequeue();
            }
            //string json = JsonSerializer.Serialize(msg);
            await writer.WriteLineAsync(msg);
        }
    }

    public static async Task GameLoop(TcpClient client)
    {
        Task dequeue = GameLoopDequeue(client);
        Task enqueue = GameLoopEnqueue(client); //tutaj są kładzione akcje gracze zarówno na InQueue - akcje do wykonania
        //przez gracza jak i na OutQueue - akcje do wykonania przez innych graczy i serwer
        await Task.WhenAll(dequeue, enqueue);
    }
    
    public static async Task GameLoopDequeue(TcpClient client)
    {
        while (true)
        {
            await InSemaphore.WaitAsync();
            lock (InQueueLock)
            {
                if (InQueue.Count > 0)
                {
                    var msg  = InQueue.Dequeue();
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

    public static async Task GameLoopEnqueue(TcpClient client)
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
                lock (InQueueLock)
                {
                    InQueue.Enqueue(msg, 1);
                }
                lock (OutQueueLock)
                {
                    OutQueue.Enqueue(read, 1);
                }
                OutSemaphore.Release();
                InSemaphore.Release();
            }
            else
            {
                lock (OutQueueLock)
                {
                    OutQueue.Enqueue(read, 1);
                }
                OutSemaphore.Release();
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