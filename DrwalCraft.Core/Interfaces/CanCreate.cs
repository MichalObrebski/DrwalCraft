namespace DrwalCraft.Core.Interfaces;

public interface ICanCreate{
    public GameObject? ObjectInProduction {get; set;}
    public bool InProduction {get;}
    public int ProductionTime {get;}
    public int ProductionProgress {get;}
    public List<ItemToCreate> Products {get;}
    public void Create(ItemToCreate product);
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