using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Messages;

namespace DrwalCraft.Client;

public class Client
{
    const int Port = 5000;
    private readonly PriorityQueue<Message, int> InQueue;
    private readonly PriorityQueue<Message, int> OutQueue;
    private readonly Lock InQueueLock;
    private readonly Lock OutQueueLock;
    private readonly SemaphoreSlim OutSemaphore;
    private readonly SemaphoreSlim InSemaphore;

    public Client(PriorityQueue<Message, int> inQueue, PriorityQueue<Message, int> outQueue, Lock inQueueLock,
        Lock outQueueLock, SemaphoreSlim inSemaphore, SemaphoreSlim outSemaphore)
    {
        InQueue = inQueue;
        OutQueue = outQueue;
        InQueueLock = inQueueLock;
        OutQueueLock = outQueueLock;
        OutSemaphore =  outSemaphore;
        InSemaphore =  inSemaphore;
    }
    
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
            lock (InQueueLock) 
            { 
                InQueue.Enqueue(msg, i); 
                InSemaphore.Release();
            } 
            i++;
        }
    }

    public async Task WriteToServer(TcpClient client)
    {
        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        while (true)
        {
            await OutSemaphore.WaitAsync();
            Message msg;
            lock (OutQueueLock)
            {
                msg = OutQueue.Dequeue();
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
                lock (InQueueLock)
                {
                    InQueue.Enqueue(msg, 1);
                }
                lock (OutQueueLock)
                {
                    OutQueue.Enqueue(msg, 1);
                }
                OutSemaphore.Release();
                InSemaphore.Release();
            }
            else
            {
                msg = new Message(client.Client.LocalEndPoint.ToString(), read);
                lock (OutQueueLock)
                {
                    OutQueue.Enqueue(msg, 1);
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