using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Building{
    public Barrack() : base("Barrack.png", 3){
        Name = "Barrack";
        Products = new ();
        Products.Add(typeof(Knight));
    }
    public void Produce(Type troop){
        var (x, y) = GameMap.GetNearestEmptyField(this);
        if(troop == typeof(Knight)){
            if(x == -1){
                Console.WriteLine("Cant");return;
            }
            GameMap.AddObjectToMap(x, y, new Knight());
        }
    }
}