using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Building{
    public Barrack(Player player) : base(player, "Barrack.png", 3){
        Name = "Barrack";
        MaxHp = 500;
        Hp = 500;
        Products.Add(new ItemToCreate(typeof(Knight), 500, 120));
        Pricing.Add("Knight: 500");
        Products.Add(new ItemToCreate(typeof(Archer), 600, 180));
        Pricing.Add("Archer: 600");
        InProduction = false;
        Console.WriteLine("Id barracka" + this.Id);
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.BarrackAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
    

    public override void Create(ItemToCreate item){
        if(InProduction) return;

        _objectInProduction = item.Make(Owner);

        if(_objectInProduction != null){
            InProduction = true;
            ProductionTime = item.ProductionTime;
        }
    }
    public override void MainAction(){
        if(_objectInProduction == null) return;
        if(_productionProgress >= _productionTime){        
            if(!GameMap.TryGetNearestEmptyField(this, out var field)){
                return;
            }
            GameMap.AddObjectToMap(field.Item1, field.Item2, _objectInProduction);
            _objectInProduction = null;
            InProduction = false;
        }
        else{
            ProductionProgress++;
        }
    }
}