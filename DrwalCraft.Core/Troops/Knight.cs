namespace DrwalCraft.Core.Troops;

public class Knight : Soldier{
    public Knight(Player player) : base(player, "Knight.png"){
        _moveProgress = 0;
        _speed = 6;
        _range = 1;
        _damage = 18;
        MaxHp = 100;
        Hp = 100;
        Name = "Knight";
        Price = 500;
    }
}