using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Channels;
using Messages;

namespace DrwalCraft.Server;

public static class Program
{
    private const int Port = 5000;
    private static ConcurrentQueue<TcpClient> _serverQueue = new ConcurrentQueue<TcpClient>();
    private static Dictionary<TcpClient, Channel<Message>> _clientsQueues =  new Dictionary<TcpClient, Channel<Message>>();
    private static List<TcpClient>_clients = new List<TcpClient>(); 
    
    public static async Task Main()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
        using var listener = new TcpListener(ipEndPoint);
        
        var tasks = new List<Task>();
        tasks.Add(AcceptClients(listener, cts.Token));
        tasks.Add(Task.Run(() => ServerGameLoop(_serverQueue)));
        await Task.WhenAll(tasks);
    }
    
    private static async Task AcceptClients(TcpListener listener, CancellationToken token = default)
    {
        listener.Start(backlog: 10);
        var clientsTasks = new List<Task>();
        
        while (true)
        {
            try
            {
                TcpClient client = await listener.AcceptTcpClientAsync(token);
                _clients.Add(client);
                clientsTasks.Add(ReceiveMesFromClient(client, token));
                _clientsQueues[client] = Channel.CreateUnbounded<Message>();
                clientsTasks.Add(SendMesToClient(client, _clientsQueues[client], token));
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        listener.Stop();
        await Task.WhenAll(clientsTasks);
    }
    
    private static async Task ReceiveMesFromClient(TcpClient client, CancellationToken token = default)
    {
        Console.WriteLine("New client connected");
        try
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream);
            
            while (!token.IsCancellationRequested)
            {
                var text = await reader.ReadLineAsync(token);
                Console.WriteLine($"Received command: {text}");
                switch (text)
                {
                    case "jebac komunizm":
                        _serverQueue.Enqueue(client);
                        foreach (var cl in _clients)
                        {
                            var msg = new Message(client.Client.RemoteEndPoint.ToString(), "jebac komunizm");
                            if (cl != client)
                            {
                                await _clientsQueues[cl].Writer.WriteAsync(msg);    
                            }
                        }
                        break;
                    default:
                        await _clientsQueues[client].Writer.WriteAsync(new Message("Serwer", "NIE SLYSZAEALEM!?"));
                        break;
                }

                if (text == null)
                {
                    _clients.Remove(client);
                    _clientsQueues.Remove(client);
                };
            }
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled, which is expected during shutdown.
        }
        catch (IOException)
        {
            // Client disconnected abruptly.
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling client: {e.Message}");
        }
        finally
        {
            client.Dispose();
            Console.WriteLine("Client disconnected");
        }
    }

    private static async Task SendMesToClient(TcpClient client, Channel<Message> channel,
        CancellationToken token = default)
    {
        try
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) { AutoFlush = true };
            while (!token.IsCancellationRequested)
            {
                var msg = await channel.Reader.ReadAsync(token);
                var json = JsonSerializer.Serialize(msg);
                await writer.WriteLineAsync(json);
            }
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled, which is expected during shutdown.
        }
        catch (IOException)
        {
            // Client disconnected abruptly.
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling client: {e.Message}");
        }
        finally
        {
            client.Dispose();
            Console.WriteLine("Client disconnected");
        }
    }

    private static void ServerGameLoop(ConcurrentQueue<TcpClient> myQueue)
    {
        //Silnik samemu wykonuje obliczenia i wywołuje wszystkie akcje otrzymane od klientów
        while (true)
        {
            TcpClient client;
            if(myQueue.TryDequeue(out client))
                Console.WriteLine($"{client.Client.RemoteEndPoint.ToString()}: jebie komunizm");
            if (myQueue.IsEmpty) Task.Delay(10).Wait();
        }
    }
}