using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public abstract class Building : GameObject{
    protected bool _inProduction;
    protected Player _player;
    public bool InProduction {
        get => _inProduction;
        set{
            _inProduction = value;
            OnPropertyChanged("InProduction");
        }
    }
    protected int _productionTime;
    protected int _progress;
    public int ProductionTime{
        get => _productionTime;
        protected set{
            _productionTime = value;
            _progress = 0;
        }
    }
    public int Progress{
        get => _progress;
        set{
            _progress = value;
            OnPropertyChanged("Progress");
        }
    }
    protected GameObject? _producing;
    public GameObject? Producing{
        get => _producing;
        set{
            _producing = value;
            OnPropertyChanged("Producing");
        }
    }
    public List<Type> Products {get; set;}
    public List<string> Pricing{get; set;}
    public Building(Player player, string? Icon = null, int size=1) : base(player, Icon, size:size){
        _player = player;
    }
    public abstract void Produce(Type item);
}