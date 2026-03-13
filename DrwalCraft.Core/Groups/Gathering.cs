namespace DrwalCraft.Core.Groups;

public class Gathering : UnitsGroup{
    public Gathering(Player player) : base(player){
        _capacity = 12;
    }

    public override bool TryAddTroop(Troops.Troop troop){
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