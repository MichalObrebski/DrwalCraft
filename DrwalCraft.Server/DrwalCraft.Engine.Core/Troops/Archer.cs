namespace DrwalCraft.Core.Troops;

public class Archer : Soldier{
    public Archer(int? playerId = null, int? objectId = null) : base("Archer.png", playerId, objectId){
        _moveProgress = 0;
        _speed = 6;
        _range = 4;
        _damage = 12;
        MaxHp = 72;
        Hp = 72;
        Name = "Archer";
    }
}