using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Groups;

public abstract class Group : GameObject{
    protected readonly List<Troop> _units = new();
    protected bool _isActive;
    protected int _capacity;
    protected int _count;

    public List<Troop> Units {get => _units;}
    public int Capacity {get => _capacity;}
    public int Count {get => _count;}
    public override bool IsActive{
        get => _isActive;
        set{
            foreach(var unit in _units)
                unit.IsActive = value;
            _isActive = value;
        }
    }
    public virtual (int, int) Target{
        set{
            foreach(var unit in _units)
                unit.Target = value;
        }
    }

    public Group(Player player) : base(player){

    }

    public virtual bool TryAddTroop(Troop troop){
        if(_units.Count >= _capacity) return false;
        _units.Add(troop);
        _count += 1;
        return true;
    }
}