using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text.Json;
using DrwalCraft.Core.Troops;
using Messages;

namespace DrwalCraft.Core;

//Fakt, że klasa jest statyczna sprawia, że serwer korzystający z referencji do tego projektu jest ograniczony do 
//wywoływania jednej gry. Tak - uświadomiliśmy to sobie, ale zrobienie całej gry, będąc debilami, nie jest banalne i jeśli 
//ten komentarz istnieje to oznacza, że nie mieliśmy czasu wykonać poprawek.
public static class ExistingObjects{
    public static List<GameObject> GameObjects = new ();
    private static ConcurrentQueue<GameObject> _addQueue = new ();
    private static ConcurrentQueue<GameObject> _removeQueue = new ();
    
    //kolejki i locki do komunikacji między klientami na obiektach - 
    public static readonly PriorityQueue<Message, int> InQueue = new(); 
    public static readonly PriorityQueue<Message, int> OutQueue = new();
    public static readonly Lock InQueueLock = new();
    public static readonly Lock OutQueueLock = new();
    public static readonly SemaphoreSlim OutSemaphore = new(0);
    public static readonly SemaphoreSlim InSemaphore = new(0);

    public static void TickAction(){
        foreach(var gameObject in GameObjects){
            if(gameObject is Troops.Troop troop){
                troop.MainAction();
            }
            if(gameObject is Buildings.Building building){
                building.MainAction();
            }
        }
        //dodaj synchronizację tikową
         while (InQueue.Count > 0)
         {
             lock(InQueueLock)
             {
                 //InSemaphore.Wait();
                 DoMessage(InQueue.Dequeue());
                 //Console.WriteLine(InQueue.Count);
             }
         }
        //dodawanie obiektów również zaimplementuj przez message i kolejkę(maybe)
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

    public static void DoMessage(Message message)
    {
        string text = JsonSerializer.Serialize(message);
        Console.WriteLine($"Doing message: {text}");
        
        int id = message.Id;
        if (message.ActionType == ActionType.MoveUnit)
        {
            foreach(var gameObject in GameObjects)
                if (gameObject.Id == id)
                {
                    if (!(gameObject is Troop)) return;
                    if (message.PositionX == null ||  message.PositionY == null)
                        (gameObject as Troop).TravelTarget = null;
                    else
                    {
                        (gameObject as Troop).TravelTarget = ((int, int)?)(message.PositionX, message.PositionY);
                    }
                }
        }
    
        if (message.ActionType == ActionType.AttackUnit)
        {
            foreach(var gameObject in GameObjects)
                if (gameObject.Id == id)
                {
                    GameObject? Opponent = null;
                    foreach(var troop in GameObjects)
                        if(troop.Id == message.AttackTargetId) Opponent = troop;
                    (gameObject as Troop).AttackTarget = Opponent;
                }
        }
    }
    
    public static void Remove(GameObject gameObject)
    {
        _removeQueue.Enqueue(gameObject);
    }
    public static void Add(GameObject gameObject){
        if (gameObject is Troop troop)
        {
            troop.TravelTargetChanged += HandleTravelTargetChanged;
            troop.AttackTargetChanged += HandleAttackTargetChanged;
        }
        _addQueue.Enqueue(gameObject);
    }
    public static void HandleAttackTargetChanged(object? sender, EventArgs e)
    {
        if ((sender as GameObject) == null) return;
        int? opponentId = null;
        foreach(var gameObject in GameObjects)
            if(gameObject == (sender as Troop).AttackTarget) opponentId = gameObject.Id;
        var msg = new Message(ActionType.AttackUnit, UnitType.Soldier, (sender as GameObject).Id, opponentId);
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg,1);
            OutQueue.Enqueue(msg,1);
            OutSemaphore.Release();
            //InSemaphore.Release();
        }
    }

    public static void HandleTravelTargetChanged(object? sender, EventArgs e)
    {
        if ((sender as GameObject)==null) return;
        var msg = new Message(ActionType.MoveUnit, UnitType.Soldier, (sender as GameObject).Id,  (sender as Troop).TravelTarget);
        Console.WriteLine($"{(sender as Troop).TravelTarget}");
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg,1);
            OutQueue.Enqueue(msg,1);
            OutSemaphore.Release();
            //InSemaphore.Release();
        }
    }

}