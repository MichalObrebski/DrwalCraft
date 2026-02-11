using System.Collections.Concurrent;

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
            if(gameObject is Buildings.Barrack barrack){
                barrack.MainAction();
            }
        }
        while(_addQueue.Count > 0){
            _addQueue.TryDequeue(out var result);
            if(result is not null)
                GameObjects.Add(result);
        }
        while(_removeQueue.Count > 0){
            _removeQueue.TryDequeue(out var result);
            if(result is not null)
                GameObjects.Remove(result);
        }
    }
    public static void Remove(GameObject gameObject){
        _removeQueue.Enqueue(gameObject);
    }
    public static void Add(GameObject gameObject){
        _addQueue.Enqueue(gameObject);
    }
}