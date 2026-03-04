
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Construction : Building{
    private Builder _builder;
    protected int _productionTime;
    protected int _productionProgress;
    protected GameObject? _objectInProduction;
    public int ProductionTime{
        get => _productionTime;
        protected set{
            _productionTime = value;
            _productionProgress = 0;
        }
    }
    public int ProductionProgress{
        get => _productionProgress;
        set{
            _productionProgress = value;
            OnPropertyChanged(nameof(ProductionProgress));
        }
    }
    public GameObject? ObjectInProduction{
        get => _objectInProduction;
        set{
            _objectInProduction = value;
            OnPropertyChanged(nameof(ObjectInProduction));
        }
    }
    public Construction(Player player, GameObject building, Builder builder) : base(player, "Construction.png", 1){
        Name = "Construction";
        MaxHp = building.MaxHp;
        Size = building.Size;
        Hp = 1;
        ObjectInProduction = building;
        _builder = builder;
        ProductionTime = MaxHp;
    }

    public override void MainAction(){
        if(_objectInProduction is null) return;

        //odliczanie postępu budowy
        if(_productionProgress >= _productionTime){
            Hp = -1;//żeby konstrukcja się usunęła
            GameMap.AddObjectToMap(Position.Item1, Position.Item2, _objectInProduction);
            _builder.InProduction = false;
            //wyrzucanie budowniczego (jeśli się da) i ponowne dodanie go do ExistingObjects
            if(GameMap.TryGetNearestEmptyField(this, out var builderPosition))
                GameMap.AddObjectToMap(builderPosition.Item1, builderPosition.Item2, _builder);
        }
        else{
            Hp++;
            ProductionProgress = Hp;
        }
    }
    public override byte[] GetIconPart(int positionX, int positionY){
        return ObjectIconPart[0];
    }
}