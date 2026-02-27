
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Construction : Building{
    private Builder _builder;
    public Construction(Player player, GameObject building, Builder builder) : base(player, "Construction.png", building.Size){
        Name = "Construction";
        MaxHp = building.MaxHp;
        Hp = 1;
        ObjectInProduction = building;
        _builder = builder;
        InProduction = true;
        
        ProductionTime = MaxHp;
    }

    public override void MainAction(){
        if(_objectInProduction is null) return;

        if(_productionProgress >= _productionTime){
            Hp = -1;
            GameMap.AddObjectToMap(Position.Item1, Position.Item2, _objectInProduction);
            _builder.InProduction = false;
            if(GameMap.TryGetNearestEmptyField(this, out var builderPosition))
                GameMap.AddObjectToMap(builderPosition.Item1, builderPosition.Item2, _builder);
        }
        else{
            Hp++;
            ProductionProgress = Hp;
        }
    }

    public override void Create(ItemToCreate item){
        return;
    }

    public new byte[]? GetIconPart(int positionX, int positionY){
        if(ObjectIconPart == null) return null;
        return ObjectIconPart[0];
    }

}