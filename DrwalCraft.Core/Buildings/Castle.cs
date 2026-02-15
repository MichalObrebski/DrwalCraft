using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Castle : Building{
    public Castle(Player player) : base(player, "Castle.png", 4){
        Name = "Castle";
        MaxHp = 1024;
        Hp = 1024;
        InProduction = false;

        Products = new();
        Products.Add(typeof(Builder));
        Products.Add(typeof(Miner));
    }

        public override void Produce(Type troop){
        if(InProduction) return;
        InProduction = true;

        if(troop == typeof(Builder)){
            _producing = new Builder(_player);
            ProductionTime = 120;
        }
        if(troop == typeof(Miner)){
            _producing = new Miner(_player);
            ProductionTime = 120;
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