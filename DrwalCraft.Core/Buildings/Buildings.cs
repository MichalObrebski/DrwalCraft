using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public interface IBuilding : IGameObject{

}

public abstract class Building : GameObject, IBuilding{
    protected bool _inProduction;
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
    public Building(string Icon, int size) : base(Icon, size:size){
        
    }
    public abstract void Produce(Type item);
}