using DrwalCraft.Core.Troops;
using DrwalCraft.Core;

namespace DrwalCraft.Core.Groups;

public class Army : Group, ICanMove{
    public Army(Player player) : base(player){
        Name = "Army";
        _capacity = 12;
    }
    public override bool TryAddTroop(Troop troop){
        if(!base.TryAddTroop(troop)) return false;

        _maxHp += troop.MaxHp;
        Hp += troop.Hp;
        return true;
    }
    public override void MainAction(){
        foreach(var troop in _units){
            troop.MainAction();
        }
    }
}