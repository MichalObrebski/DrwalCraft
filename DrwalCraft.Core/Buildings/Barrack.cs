using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Building{
    public Barrack(Player player) : base(player, "Barrack.png", 3){
        Name = "Barrack";
        MaxHp = 500;
        Hp = 500;
        Products = new ();
        Pricing = new ();
        Products.Add(typeof(Knight));
        Pricing.Add("Knight: 500");
        Products.Add(typeof(Archer));
        Pricing.Add("Archer: 600");
        InProduction = false;
        Price = 2000;
        Console.WriteLine("Id barracka" + this.Id);
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.BarrackAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
    

    public override void Produce(Type troop){
        if(InProduction) return;

        if(troop == typeof(Knight)){
            if(_player.Wood >= 500){
                _player.Wood -= 500;
                _producing = new Knight(_player);
                ProductionTime = 120;
            }
        }
        if(troop == typeof(Archer)){
            if(_player.Wood >= 600){
                _player.Wood -= 600;
                ProductionTime = 180;
                _producing = new Archer(_player);
            }
        }

        if(_producing != null)
            InProduction = true;
    }
    public override void MainAction(){
        if(_producing == null) return;
        if(_progress >= _productionTime){        
            if(!GameMap.TryGetNearestEmptyField(this, out var field)){
                return;
            }
            GameMap.AddObjectToMap(field.Item1, field.Item2, _producing);
            _producing = null;
            InProduction = false;
        }
        else{
            Progress++;
        }
    }
}