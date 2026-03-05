using DrwalCraft.Core.Troops;
using DrwalCraft.Core;

namespace DrwalCraft.Core.Groups;

public class Army : UnitsGroup, ICanAttack{
    public GameObject? AttackTarget{
        get{
            //czy wszystkie jednostki się focusują na jednym celu
            var collectiveTarget = Units.First().AttackTarget;
            if(Units.All(unit => unit.AttackTarget == collectiveTarget))
                return collectiveTarget;
            //jak nie to zwraca null
            return null;
        }
    }

    public Army(Player player) : base(player){
        Name = "Army";
        _capacity = 12;
    }

    public event EventHandler? AttackTargetChanged;

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