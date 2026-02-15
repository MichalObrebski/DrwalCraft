using System.Collections.Concurrent;
using System.Text.Json;
using DrwalCraft.Core.Buildings;
using DrwalCraft.Core.Troops;
using DrwalCraft.DrwalCraftCore.GameLoop;
using Messages;

namespace DrwalCraft.Core;

public static class ExistingObjects{
    public static List<GameObject> GameObjects = new ();
    private static ConcurrentQueue<GameObject> _addQueue = new ();
    private static ConcurrentQueue<GameObject> _removeQueue = new ();
    
    public static void TickAction(){
        foreach(var gameObject in GameObjects){
            if(gameObject is Troops.Troop troop){
                troop.MainAction();
            }
            if(gameObject is Buildings.Building building){
                building.MainAction();
            }
        }
        
         while (ObjectsActions.InQueue.Count > 0) 
         {
             lock(ObjectsActions.InQueueLock)
             {
                 var msg =  ObjectsActions.InQueue.Dequeue();
                 if (msg.Tick > GameLoop.CurrentTick)
                 {
                     ObjectsActions.InQueue.Enqueue(msg, msg.Tick);
                     break;
                 }
                 
                 ObjectsActions.DoMessage(msg);
             }
         }
        
        while(_addQueue.Count > 0){
            _addQueue.TryDequeue(out var result);
            if (result is not null)
            {
                GameObjects.Add(result);
            }
        }
        while(_removeQueue.Count > 0){
            _removeQueue.TryDequeue(out var result);
            if(result is not null)
                GameObjects.Remove(result);
        }
    }
    
    public static void Remove(GameObject gameObject)
    {
        _removeQueue.Enqueue(gameObject);
    }
    
    public static void Add(GameObject gameObject){
        Console.WriteLine($"Adding object of id: {gameObject.Id}");
        if (gameObject is Troop troop)
        {
            troop.TravelTargetChanged += ObjectsActions.HandleTravelTargetChanged;
        }

        if (gameObject is Soldier soldier)
        {
            soldier.AttackTargetChanged += ObjectsActions.HandleAttackTargetChanged;
        }

        if (gameObject is Miner miner)
        {
            miner.TargetMineChanged += ObjectsActions.HandleMineTargetChanged;
        }
        
        _addQueue.Enqueue(gameObject);
    }
}