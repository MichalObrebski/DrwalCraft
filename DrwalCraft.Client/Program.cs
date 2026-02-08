using DrwalCraft.Game;
using DrwalCraft.Client;
using Messages;

public static class Program
{
    private static readonly PriorityQueue<Message,int> InQueue = new PriorityQueue<Message, int>();
    private static readonly PriorityQueue<string,int> OutQueue = new PriorityQueue<string, int>();
    private static readonly Lock InQueueLock = new Lock();
    private static readonly Lock OutQueueLock = new Lock();
    private static readonly SemaphoreSlim OutSemaphore = new SemaphoreSlim(0);
    private static readonly SemaphoreSlim InSemaphore = new SemaphoreSlim(0);
    
    [STAThread]
    public static void Main(string[] args)
    {
        var client = new Client(InQueue, OutQueue, InQueueLock, OutQueueLock, InSemaphore, OutSemaphore);
        Task network = Task.Run(()=> client.ConnectAsync());
        var game = new Game(InQueue, OutQueue, InQueueLock, OutQueueLock, InSemaphore, OutSemaphore);
        Game.Main();
        network.Wait();
    }
    
}