using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        var stream =  client.GetStream();    
        
        var startLengthBuffer = new byte[sizeof(Int32)];
        var startBytesRead = await ReadToBuffer(stream, startLengthBuffer);
        if (startBytesRead < sizeof(Int32)) // Disconnection from the server
            return;
        var startlength = BinaryPrimitives.ReadInt32LittleEndian(startLengthBuffer);
            
        var startPayloadBuffer = new byte[startlength];
        startBytesRead = await ReadToBuffer(stream, startPayloadBuffer);
        if (startBytesRead < startlength)
            return;
        
        string startJson = Encoding.UTF8.GetString(startPayloadBuffer);
        var startMsg = JsonSerializer.Deserialize<Message>(startJson);
        if (startMsg is null) return;
        Console.WriteLine(startJson);
        
        string playerId = startMsg.Text;
        Players.game = new Player(1);
        if (playerId == 3.ToString())
        {
            Players.you = new Player(3);
            Players.enemy = new Player(2);
        }
        if(playerId == 2.ToString())
        {
            Players.you = new Player(2);
            Players.enemy = new Player(3);
        }
        
        tcs.SetResult(true); // sygnał o rozpoczęciu gry (drugi klient podłączył się)
        
        while (true)
        {
            var typeBuffer = new byte[1];
            int bytesRead = await ReadToBuffer(stream, typeBuffer);
            if (bytesRead < 1) // Disconnection from the server
                return;
            var type = (Messages.MessageType)typeBuffer[0];

            var lengthBuffer = new byte[sizeof(Int32)];
            bytesRead = await ReadToBuffer(stream, lengthBuffer);
            if (bytesRead < sizeof(Int32)) // Disconnection from the server
                return;
            var length = BinaryPrimitives.ReadInt32LittleEndian(lengthBuffer);
            
            var payloadBuffer = new byte[length];
            bytesRead = await ReadToBuffer(stream, payloadBuffer);
            if (bytesRead < length)
                return;
                        
            if (type == MessageType.PlayerAction)
            {
                string json = Encoding.UTF8.GetString(payloadBuffer);
                var msg = JsonSerializer.Deserialize<Message>(json);
                if (msg is null) break;
                Console.WriteLine(json);
                
                lock (ObjectsActions.InQueueLock) 
                { 
                    ObjectsActions.InQueue.Enqueue(msg, msg.Tick); 
                    //ExistingObjects.InSemaphore.Release();
                } 
            }
            
        }
    }

    private async Task<int> ReadToBuffer(Stream stream, byte[] buffer)
    {
        int bytesRead = 0; int chunkSize = 1;

        while (bytesRead < buffer.Length && chunkSize > 0)
        {
            chunkSize = await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
            bytesRead += chunkSize;
        }

        return bytesRead;
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
    
}