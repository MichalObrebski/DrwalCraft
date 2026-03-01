using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public abstract class Building : GameObject, ICanCreate{
    protected bool _inProduction;
    protected SpinLock _productionLock = new();
    public bool InProduction {
        get => _inProduction;
        set{
            _inProduction = value;
            OnPropertyChanged(nameof(InProduction));
        }
    }
    protected int _productionTime;
    protected int _productionProgress;
    public int ProductionTime{
        get => _productionTime;
        protected set{
            _productionTime = value;
            _productionProgress = 0;
        }
    }
    public int ProductionProgress{
        get => _productionProgress;
        set{
            _productionProgress = value;
            OnPropertyChanged(nameof(ProductionProgress));
        }
    }
    protected GameObject? _objectInProduction;
    public GameObject? ObjectInProduction{
        get => _objectInProduction;
        set{
            _objectInProduction = value;
            OnPropertyChanged(nameof(ObjectInProduction));
        }
    }
    public List<ItemToCreate> Products {get; set;}
    public Building(Player player, string? Icon = null, int size=1) : base(player, Icon, size:size){
        Products = new();
    }
    public abstract void Create(ItemToCreate item);
}