using System.Collections.Concurrent;
using System.Text.Json;
using DrwalCraft.Core.Buildings;
using DrwalCraft.Core.Troops;
using DrwalCraft.DrwalCraftCore.GameLoop;

using Messages;


namespace DrwalCraft.Core;

public static class ObjectsActions
{
    //kolejki i locki do komunikacji między klientami na obiektach - 
    public static readonly PriorityQueue<Message, int> InQueue = new(); 
    public static readonly PriorityQueue<Message, int> OutQueue = new();
    public static readonly Lock InQueueLock = new();
    public static readonly Lock OutQueueLock = new();
    public static readonly SemaphoreSlim OutSemaphore = new(0);
    public static readonly SemaphoreSlim InSemaphore = new(0);
    
    public static void DoMessage(Message message)
    {
        string text = JsonSerializer.Serialize(message);
        Console.WriteLine($"Doing message: {text} on tik {GameLoop.CurrentTick}");
        
        int id = message.Id;
        if (message.ActionType == ActionType.MoveUnit)
        {
            foreach(var gameObject in ExistingObjects.GameObjects)
                if (gameObject.Id == id)
                {
                    if (!(gameObject is Troop)) return;
                    if (message.PositionX == null ||  message.PositionY == null)
                        (gameObject as Troop).SetQueuedTravelTarget(null);
                    else
                    {
                        Console.WriteLine(message.PositionX + ":" + message.PositionY);
                        (gameObject as Troop).SetQueuedTravelTarget(((int, int)?)(message.PositionX, message.PositionY));
                        //Console.WriteLine((gameObject as Troop).);
                    }
                }
        }
    
        if (message.ActionType == ActionType.AttackUnit)
        {
            foreach(var gameObject in ExistingObjects.GameObjects)
                if (gameObject.Id == id)
                {
                    GameObject? Opponent = null;
                    foreach(var troop in ExistingObjects.GameObjects)
                        if(troop.Id == message.AttackTargetId) Opponent = troop;
                    (gameObject as Troop).SetQueuedAttackTarget(Opponent);
                }
        }

        if (message.ActionType == ActionType.CreateUnitBarrack)
        {
            foreach (var gameObject in ExistingObjects.GameObjects)
            {
                if (gameObject is Barrack barrack)
                {
                    barrack.Produce(message.UnitType == UnitType.Knight?typeof(Knight):typeof(Archer));
                }
            }
        }
    }
    
     public static void HandleAttackTargetChanged(object? sender, EventArgs e)
    {
        if ((sender as GameObject) == null) return;
        int? opponentId = null;
        foreach(var gameObject in ExistingObjects.GameObjects)
            if(gameObject == (sender as Troop)._queuedAttackTarget) opponentId = gameObject.Id;
        int key = GameLoop.CurrentTick + GameLoop.OffsetTik;
        var msg = new Message(ActionType.AttackUnit, UnitType.Soldier, (sender as GameObject).Id, opponentId, key);
        Console.WriteLine($"Enqueing message on tik {GameLoop.CurrentTick}");
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg, key);
            //InSemaphore.Release();
        }

        lock (OutQueueLock)
        {
            OutQueue.Enqueue(msg, key);
            OutSemaphore.Release();
        }
    }

    public static void HandleTravelTargetChanged(object? sender, EventArgs e)
    {
        if ((sender as GameObject)==null) return;
        int key = GameLoop.CurrentTick + GameLoop.OffsetTik;
        var msg = new Message(ActionType.MoveUnit, UnitType.Soldier, (sender as GameObject).Id,  (sender as Troop)._queuedTravelTarget, key);
        Console.WriteLine($"Enqueing message on tik {GameLoop.CurrentTick}");
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg,key);
            //InSemaphore.Release();
        }
        lock (OutQueueLock)
        {
            OutQueue.Enqueue(msg,key);
            OutSemaphore.Release();
        }
    }

    public static void BarrackAddMessage(Type Troop)
    {
        int key = GameLoop.CurrentTick + GameLoop.OffsetTik;
        var msg = new Message(ActionType.CreateUnitBarrack, Troop == typeof(Knight)?UnitType.Knight:UnitType.Archer, key);
        Console.WriteLine($"Enqueing message on tik {GameLoop.CurrentTick}");
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg, key);
        }

        lock (OutQueueLock)
        {
            OutQueue.Enqueue(msg, key);
            OutSemaphore.Release();
        }
    }
}