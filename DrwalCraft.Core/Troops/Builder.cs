using DrwalCraft.Core.Buildings;

namespace DrwalCraft.Core.Troops;

public class Builder : Troop{
    public List<Type> Products {get; set;}
    private Building? _inConstruction;
    public bool Constructing {set; get;}
    public Builder(int? playerId = null, int? objectId = null) : base("Builder.png", playerId, objectId){
        Name = "Builder";
        MaxHp = 50;
        Hp = 50;
        Constructing = false;
        Products = new();

        Products.Add(typeof(Barrack));
    }

    public override void MainAction(){
        
    }
    public void Build(Type building){
        if(Constructing) return;
        Constructing = true;

        if(building == typeof(Barrack)){
            _inConstruction = new Barrack();
        }

        if(_inConstruction is null) return;

        int size = _inConstruction.Size;
        int x = -1, y = -1;
        for(int i = Position.Item1 - size + 1; i < Position.Item1 + size && x == -1; i++)
            for(int j = Position.Item2 - size + 1; j < Position.Item2 + size && x == -1; j++)
                if(IsAbleToBuild(i, j, size)){
                    x = i;
                    y = j;
                }
        GameMap.AddObjectToMap(x, y, new Construction(_inConstruction, this));
    }
    private bool IsAbleToBuild(int x, int y, int size){
        for(int i = x; i < x+size; i++){
            for(int j = y; j < y+size; j++){
                if(GameMap.Map[i,j].GameObject != null && GameMap.Map[i,j].GameObject != this)
                    return false;
            }
        }
        return true;
    }
}