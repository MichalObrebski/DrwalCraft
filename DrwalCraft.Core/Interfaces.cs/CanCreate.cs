namespace DrwalCraft.Core;

public interface ICanCreate{
    public abstract GameObject? ObjectInCreation {get; set;}
    public abstract bool InProduction {get;}
    public abstract int ProductionTime {get;}
    public abstract int ProductionProgress {get;}
    public abstract List<GameObject> Products {get;}
    public abstract void Create();
}