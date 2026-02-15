using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Building{
    public Barrack(Player player) : base(player, "Barrack.png", 3){
        Name = "Barrack";
        MaxHp = 500;
        Hp = 500;
        Products = new ();
        Products.Add(typeof(Knight));
        Products.Add(typeof(Archer));
        InProduction = false;
        Console.WriteLine("Id barracka" + this.Id);
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.BarrackAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
    

    public override void Produce(Type troop){
        if(InProduction) return;
        InProduction = true;

        if(troop == typeof(Knight)){
            _producing = new Knight(_player);
            ProductionTime = 120;
        }
        if(troop == typeof(Archer)){
            ProductionTime = 180;
            _producing = new Archer(_player);
        }
    }
    public override void MainAction(){
        if(_producing == null) return;
        if(_progress >= _productionTime){
            var (x, y) = GameMap.GetNearestEmptyField(this);
            if(x == -1){
                return;
            }
            GameMap.AddObjectToMap(x, y, _producing);
            _producing = null;
            InProduction = false;
        }
        else{
            Progress++;
        }
    }
}