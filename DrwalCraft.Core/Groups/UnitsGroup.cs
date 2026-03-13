using System.Text.RegularExpressions;
using DrwalCraft.Core.Troops;
using DrwalCraft.Core.Interfaces;

namespace DrwalCraft.Core.Groups;

public abstract class UnitsGroup : GameObject, ICanReceiveTarget{
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

    public UnitsGroup(Player player) : base(player){

    }
    /// <summary>
    /// Creates instance of unit type specific Group if all units are the same type or Gathering if units are different types of there is no type specific Group
    /// </summary>
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
                return new Gathering(Players.you);

        //utworzenie i zwrócenie grupy odpowiedniej dla danego typu jednostek
        Troop representative = units.First();
        return representative switch{
            Soldier => new Army(Players.you),
            _ => new Gathering(Players.you)
        };
    }
    public virtual bool TryAddTroop(Troop troop){
        if(_units.Count >= _capacity) return false;
        _units.Add(troop);
        _count += 1;
        return true;
    }
}