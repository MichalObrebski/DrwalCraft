using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using DrwalCraft.Core;
using Messages;
using System.Diagnostics;

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
        await Task.WhenAll(tasks);
    }
    
    private static async Task AcceptClients(TcpListener listener, CancellationToken token = default)
    {
        listener.Start(backlog: 2);
        Console.WriteLine($"Listening on port {Port}");
        var Tasks = new List<Task>();
        for (int i = 0; i < 2; i++)
        {
            try 
            {
                TcpClient client = await listener.AcceptTcpClientAsync(token);
                _clients.Add(client); 
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        Task Game = Task.Run(()=>
        {
            ServerGame.Game();
        }, token);
        Tasks.Add(Game);
        
        foreach(var client in _clients)
        {
            Tasks.Add(ReceiveMesFromClient(client, token));
            _clientsQueues[client] = Channel.CreateUnbounded<Message>();
            Tasks.Add(SendMesToClient(client, _clientsQueues[client], token));
        }
        
        listener.Stop();
        await Task.WhenAll(Tasks);
        
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
                var msg = JsonSerializer.Deserialize(text, typeof(Message)) as Message; 
                Console.WriteLine($"Received command: {text}");
                
                lock (ObjectsActions.InQueueLock)
                {
                    ObjectsActions.InQueue.Enqueue(msg, msg.Tick);
                }
                
                msg.From = "Serwer";
                
                foreach (var cl in _clients) 
                {
                    if (cl != client)
                    {
                        await _clientsQueues[cl].Writer.WriteAsync(msg);    
                    }
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
            
            var startMsg = new Message("Serwer", "Start");
            var startJson = JsonSerializer.Serialize(startMsg);
            var startPayload = Encoding.UTF8.GetBytes(startJson);
            byte[] startLengthHeader = new byte[sizeof(Int32)];
            BinaryPrimitives.WriteInt32LittleEndian(startLengthHeader,startPayload.Length); 
            
            await stream.WriteAsync(startLengthHeader, token);
            await stream.WriteAsync(startPayload, token);
            
            await stream.FlushAsync(token);
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (!token.IsCancellationRequested)
            {
                
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    //sending snapshot of a map
                    
                    
                    
                    
                    stopwatch.Restart();
                }
                var msg = await channel.Reader.ReadAsync(token);
                var json = JsonSerializer.Serialize(msg);
                byte[] payload = Encoding.UTF8.GetBytes(json);
                
                byte headerType = (byte)(Messages.MessageType.PlayerAction);
                var length = payload.Length;
                byte[] headerLength = new byte[sizeof(Int32)];
                BinaryPrimitives.WriteInt32LittleEndian(headerLength, length);
                Console.WriteLine($"Serializing message: json: {json}");
                
                await stream.WriteAsync(new[]{headerType}, token);
                await stream.WriteAsync(headerLength, token);
                await stream.WriteAsync(payload, token);
                await stream.FlushAsync(token);
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
    
}