using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Castle : Building{
    public Castle(Player player) : base(player, "Castle.png", 4){
        Name = "Castle";
        MaxHp = 1024;
        Hp = 1024;
        InProduction = false;

        Products = new();
        Pricing = new();
        Products.Add(typeof(Builder));
        Pricing.Add("Builder: 300");
        Products.Add(typeof(Miner));
        Pricing.Add("Miner: 300");
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.CastleAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
        public override void Produce(Type troop){
        if(InProduction) return;

        if(troop == typeof(Builder)){
            if(_player.Wood >= 300){
                _player.Wood -= 300;
                _producing = new Builder(_player);
                ProductionTime = 120;
            }
        }
        if(troop == typeof(Miner)){
            if(_player.Wood >= 300){
                _player.Wood -= 300;
                _producing = new Miner(_player);
                ProductionTime = 120;
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