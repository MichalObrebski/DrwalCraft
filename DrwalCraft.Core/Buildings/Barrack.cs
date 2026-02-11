using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Building{
    public bool InProduction {set; get;}
    private int _productionTime;
    private GameObject? _producing;
    public Barrack() : base("Barrack.png", 3){
        Name = "Barrack";
        MaxHp = 500;
        Hp = 500;
        Products = new ();
        Products.Add(typeof(Knight));
        Products.Add(typeof(Archer));
        InProduction = false;
    }
    public void Produce(Type troop){
        if(InProduction) return;
        InProduction = true;

        if(troop == typeof(Knight)){
            _producing = new Knight();
            _productionTime = 120;
        }
        if(troop == typeof(Archer)){
            _productionTime = 180;
            _producing = new Archer();
        }
    }
    public void MainAction(){
        if(_producing == null) return;
        if(_productionTime == 0){
            var (x, y) = GameMap.GetNearestEmptyField(this);
            if(x == -1){
                Console.WriteLine("Can't");
                return;
            }
            GameMap.AddObjectToMap(x, y, _producing);
            _producing = null;
            InProduction = false;
        }
        else{
            _productionTime--;
        }
    }
}