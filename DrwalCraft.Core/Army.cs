using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Army;

public class Army : GameObject{
    private bool _isActive;
    public List<Troop> Troops{get; set;}
    public override bool IsActive{
        get => _isActive;
        set{
            foreach(var troop in Troops)
                troop.IsActive = value;
            _isActive = value;
        }
    }

    public Army(Player player) : base(player){
        Troops = new List<Troop>();
        Name = "Army";
    }
    public bool TryAddTroop(Troop troop){
        if(Troops.Count >= 12) return false;

        Troops.Add(troop);
        _maxHp += troop.MaxHp;
        Hp += troop.Hp;
        return true;
    }
    public override void MainAction(){
        foreach(var troop in Troops){
            troop.MainAction();
        }
    }
}