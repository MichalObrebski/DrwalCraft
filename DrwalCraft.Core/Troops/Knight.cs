namespace DrwalCraft.Core.Troops;

public class Knight : Soldier{
    public Knight(int? playerId = null, int? objectId = null) : base("Knight.png", playerId, objectId){
        _moveProgress = 0;
        _speed = 6;
        _range = 1;
        _damage = 16;
        MaxHp = 50;
        Hp = 50;
        Name = "Knight";
    }
}