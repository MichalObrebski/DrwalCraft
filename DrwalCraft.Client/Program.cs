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
        Task network = Task.Run(()=> client.ConnectAsync());
        var game = new Game();
        Game.Main();
        network.Wait();
    }
    
}