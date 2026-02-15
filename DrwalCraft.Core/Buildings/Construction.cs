
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Construction : Building{
    private Builder _builder;
    public Construction(Player player, GameObject building, Builder builder) : base(player, "Construction.png", building.Size){
        Name = "Construction";
        MaxHp = building.MaxHp;
        Hp = 1;
        Producing = building;
        _builder = builder;
        InProduction = true;
        
        ProductionTime = MaxHp;
    }

    public override void MainAction(){
        if(_producing is null) return;

        if(_progress >= _productionTime){
            var builderPosition = GameMap.GetNearestEmptyField(this);
            Hp = -1;
            GameMap.AddObjectToMap(Position.Item1, Position.Item2, _producing);
            _builder.Constructing = false;
            GameMap.AddObjectToMap(builderPosition.Item1, builderPosition.Item2, _builder);
        }
        else{
            Hp++;
            Progress = Hp;
        }
    }

    public override void Produce(Type item){
        return;
    }

    public new byte[]? GetIconPart(int positionX, int positionY){
        if(ObjectIconPart == null) return null;
        return ObjectIconPart[0];
    }
}