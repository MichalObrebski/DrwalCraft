using DrwalCraft.DrwalCraftCore.GameLoop;
using DrwalCraft.Core;
using DrwalCraft.Core.Troops;
using DrwalCraft.Core.Army;

namespace DrwalCraft.Server;

public class ServerGame
{
    public static DrwalCraft.Core.Troops.Soldier? soldier1;
    public static DrwalCraft.Core.Troops.Soldier? soldier2;
    public static DrwalCraft.Core.Troops.Soldier? soldier3;
    
    public static void Game()
    {
        GameLoop.UpdateGameLogic = (() => { ExistingObjects.TickAction(); });
        var mapLock = new ReaderWriterLockSlim();
        GameMap.Init(64);
        DrwalCraft.Core.GameObjectId.Init(1);
        
        soldier1 = new DrwalCraft.Core.Troops.Knight();
        DrwalCraft.Core.GameMap.AddObjectToMap(16, 16, soldier1);

        soldier2 = new Knight(5, 10);
        DrwalCraft.Core.GameMap.AddObjectToMap(12, 8, soldier2);

        // soldier2.TravelTarget = (20,30);

        soldier3 = new DrwalCraft.Core.Troops.Knight();

        DrwalCraft.Core.GameMap.AddObjectToMap(1, 2, new Builder());
        DrwalCraft.Core.GameMap.AddObjectToMap(2, 2,  new DrwalCraft.Core.Tree());
        DrwalCraft.Core.GameMap.AddObjectToMap(28, 24, new DrwalCraft.Core.Buildings.Castle());
        DrwalCraft.Core.GameMap.AddObjectToMap(24, 14, new DrwalCraft.Core.Mines.Mine());
        DrwalCraft.Core.GameMap.AddObjectToMap(20, 14, new DrwalCraft.Core.Troops.Miner());
        DrwalCraft.Core.GameMap.AddObjectToMap(16, 20, new DrwalCraft.Core.Buildings.Barrack());
        GameLoop.StartGameLoop(mapLock);

        Task.Delay(100).Wait();
        Console.WriteLine(ExistingObjects.GameObjects.Count);
        foreach(var gameObject in ExistingObjects.GameObjects)
        {
            Console.WriteLine(gameObject.Name + gameObject.Id + gameObject.Position);
        }
        
        Task.Delay(TimeSpan.FromSeconds(30)).Wait();
        Console.WriteLine(ExistingObjects.GameObjects.Count);
        foreach(var gameObject in ExistingObjects.GameObjects)
        {
            Console.WriteLine(gameObject.Name + gameObject.Id + gameObject.Position);
        }
        
        Task.Delay(TimeSpan.FromSeconds(30)).Wait();
        Console.WriteLine(ExistingObjects.GameObjects.Count);
        foreach(var gameObject in ExistingObjects.GameObjects)
        {
            Console.WriteLine(gameObject.Name + gameObject.Id + gameObject.Position);
        }
    }
}