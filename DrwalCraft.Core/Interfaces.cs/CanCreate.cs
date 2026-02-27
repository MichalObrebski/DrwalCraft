namespace DrwalCraft.Core;

public interface ICanCreate{
    public abstract GameObject? ObjectInProduction {get; set;}
    public abstract bool InProduction {get;}
    public abstract int ProductionTime {get;}
    public abstract int ProductionProgress {get;}
    public abstract List<ItemToCreate> Products {get;}
    public abstract void Create(ItemToCreate product);
}

public class ItemToCreate{
    public Type Item {init; get;}
    public int PriceWood {set; get;}
    public int ProductionTime {set; get;}
    public ItemToCreate(Type item, int wood, int time){
        Item = item;
        PriceWood = wood;
        ProductionTime = time;
    }
    public GameObject? Make(Player player){
        if(player.Wood < PriceWood) return null;

        var constructor = Item.GetConstructor([typeof(Player)]);
        GameObject? item = constructor?.Invoke([player]) as GameObject;
        return item;
    }
}