using System.ComponentModel;
using System.IO;


namespace DrwalCraft.Core;

public enum GameObjectFor
{
    Army,
}
public static class GameObjectId{
    private static int _player;
    private static int _objectCount;
    public static int PlayerId {get => _player;}
    private static bool IsPrime(int n){
        if(n <= 2) return false;
        if(n % 2 == 0) return false;
        for(int i=3; i*i <= n; i+=2){
            if(n % i == 0) return false;
        }
        return true;
    }
    private static int NthOddPrime(int n){
        int count = 0;
        int num = 1;
        while(count < n){
            num++;
            if(IsPrime(num))
                count ++;
        }
        return num;
    }
    public static void Init(int player){
        _player = NthOddPrime(player);
        _objectCount = 1;
    }
    public static int GetNewId(){
        _objectCount++;
        Console.WriteLine($"player:{_player}, count:{_objectCount}");
        return _objectCount * 2 * _player;
    }
}
public interface IGameObject{
    public int Id {get;}
    public (int, int) Position {get;}
}
public abstract class GameObject : IGameObject{
    protected int _hp;
    public int Id {init; get;}
    public int PlayerId {init; get;}
    public (int, int) Position {set; get;}
    public int Size {set; get;}
    public int Hp{
        get => _hp;
        set{
            _hp = value;
            if(_hp <= 0){
                GameMap.Map[Position.Item1, Position.Item2].SetDefault();
                IsDead = true;
                ExistingObjects.Remove(this);
            }
        }
    }
    public virtual void GetAttacked(int damage, GameObject attacker){
        Hp -= damage;
    }
    public bool IsDead;
    public int MaxHp{set; get;}
    public string Name{init; get;}
    public virtual bool IsActive{set; get;}

    //konstruktor do armii, zeby nie zwiekszac liczby obiektów dla determinizmu ID jednostek
    public GameObject(GameObjectFor X)
    {
        if (X != GameObjectFor.Army)
            return;
        PlayerId = -1;

        Size = 1;
    }
    public GameObject(string? Icon = null, int? playerId = null, int? objectId = null, int size = 1){
        if(playerId is null)
            PlayerId = GameObjectId.PlayerId;
        else
            PlayerId = playerId.Value;
        if(PlayerId == GameObjectId.PlayerId)
            Id = GameObjectId.GetNewId();
        else
            Id = objectId ?? 1;
        Size = size;
        IsDead = false;
    }
}

public class Tree: GameObject{
    public Tree() : base("Tree.png"){
        Name = "Tree";
    }
}