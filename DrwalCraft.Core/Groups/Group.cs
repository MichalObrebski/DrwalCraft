using System.Text.RegularExpressions;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Groups;

public abstract class UnitsGroup : GameObject, ICanMove{
    protected readonly List<Troop> _units = new();
    protected bool _isActive;
    protected int _capacity;
    protected int _count;
    protected (int, int)? _travelTarget;

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
            _travelTarget = value;
            foreach(var unit in _units)
                unit.Target = value;
        }
    }
    public (int, int)? TravelTarget{get => _travelTarget;}
    public event EventHandler? TravelTargetChanged;

    public UnitsGroup(Player player) : base(player){

    }
    /// <param name="units">A non-empty list of Units which shall form a UnitsGroup</param>
    /// <returns>
    /// A UnitsGroup derived class instance based on units type
    /// </returns>
    /// <exception cref="ArgumentException"/>
    public static UnitsGroup GetGroup(List<Troop> units){
        if(units.Count == 0)
            throw new ArgumentException("'units' list is empty");
        //sprawdzenie czy jednostki są jednakowego typu
        Type type = units.First().GetType();
        foreach(var unit in units)
            if(unit.GetType() != type) //jeśli nie są to zwraca
                throw new NotImplementedException();

        //utworzenie i zwrócenie grupy odpowiedniej dla danego typu jednostek
        Troop representative = units.First();
        return representative switch{
            Soldier => new Army(Players.you),
            _ => throw new NotImplementedException()
        };
    }
    public virtual bool TryAddTroop(Troop troop){
        if(_units.Count >= _capacity) return false;
        _units.Add(troop);
        _count += 1;
        return true;
    }
}