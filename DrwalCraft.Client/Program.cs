using DrwalCraft.Game;
using DrwalCraft.Client;
using DrwalCraft.Core;
using Messages;

public static class Program
{
    
    [STAThread]
    public static void Main(string[] args)
    {
        var client = new Client();
        Task<bool> isOtherClientConnected = client.tcs.Task;
        Task network = Task.Run(()=> client.ConnectAsync());
        var game = new Game();
        Console.WriteLine("Wiating for another player to join...");
        bool temp = isOtherClientConnected.Result;
        Console.WriteLine(DateTime.Now.ToLongTimeString());
        Game.Main();
        network.Wait();
    }
    
}