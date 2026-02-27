using System.Windows.Documents;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Castle : Building{
    public Castle(Player player) : base(player, "Castle.png", 4){
        Name = "Castle";
        MaxHp = 1024;
        Hp = 1024;
        InProduction = false;

        Products.Add(new ItemToCreate(typeof(Builder), 300, 120));
        Pricing.Add("Builder: 300");
        Products.Add(new ItemToCreate(typeof(Miner), 300, 120));
        Pricing.Add("Miner: 300");
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.CastleAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
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