using System.Collections.Concurrent;
using System.ComponentModel;
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
    private static readonly PriorityQueue<Message, int> InQueue = new(); 
    private static readonly PriorityQueue<Message, int> OutQueue = new();
    private static readonly Lock InQueueLock = new();
    private static readonly Lock OutQueueLock = new();
    private static readonly SemaphoreSlim OutSemaphore = new(0);
    private static readonly SemaphoreSlim InSemaphore = new(0);

    public static void TickAction(){
        foreach(var gameObject in GameObjects){
            if(gameObject is Troops.Troop troop){
                troop.MainAction();
            }
        }
        //dodaj synchronizację tikową
         while (InQueue.Count > 0)
         {
             lock(InQueueLock)
             {
                 //InSemaphore.Wait();
                 DoMessage(InQueue.Dequeue());
                 Console.WriteLine(InQueue.Count);
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
        int id = message.Id;
        if (message.ActionType == ActionType.MoveUnit)
        {
            foreach(var gameObject in GameObjects)
                if (gameObject.Id == id)
                    (gameObject as Troop).TravelTarget = message.Position;
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
            //InSemaphore.Release();
        }
    }

    public static void HandleTravelTargetChanged(object? sender, EventArgs e)
    {
        if ((sender as GameObject)==null) return;
        var msg = new Message(ActionType.MoveUnit, UnitType.Soldier, (sender as GameObject).Id,  (sender as Troop).TravelTarget);
        lock (InQueueLock)
        {
            InQueue.Enqueue(msg,1);
            //InSemaphore.Release();
        }
    }

}