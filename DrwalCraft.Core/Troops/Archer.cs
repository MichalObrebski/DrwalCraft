namespace DrwalCraft.Core.Troops;

public class Archer : Soldier{
    public Archer(Player player) : base(player, "Archer.png"){
        _moveProgress = 0;
        _speed = 6;
        _range = 4;
        _damage = 12;
        MaxHp = 72;
        Hp = 72;
        Name = "Archer";
        Price = 600;
    }
}