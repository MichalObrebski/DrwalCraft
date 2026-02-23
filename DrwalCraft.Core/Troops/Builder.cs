using DrwalCraft.Core.Buildings;

namespace DrwalCraft.Core.Troops;

public class Builder : Troop{
    public List<Type> Products {get; set;}
    public List<string> Pricing {get; set;}
    private Building? _inConstruction;
    public bool Constructing {set; get;}
    public override (int, int) Target {
        set => TravelTarget = value;
    }

    public Builder(Player player) : base(player, "Builder.png"){
        Name = "Builder";
        MaxHp = 50;
        Hp = 50;
        Constructing = false;
        _speed = 8;
        Products = new();
        Pricing = new();
        Price = 300;

        Products.Add(typeof(Barrack));
        Pricing.Add("Barrack: 2000");
    }

    public override void MainAction(){
        if(TravelTarget != null)
            Move();
    }

    public void DoMessage(Type building)
    {
        ObjectsActions.BuildAddMessage(this.Id, _inConstruction);
    }
    public void Build(Type building){
        if(Constructing) return;
        Constructing = true;

        if(building == typeof(Barrack)){
            if(Owner.Wood >= 2000){
                Owner.Wood -= 2000;
                _inConstruction = new Barrack(Owner);
            }
        }

        if(_inConstruction is null) return;

        int size = _inConstruction.Size;
        int x = -1, y = -1;
        for(int i = Position.Item1 - size + 1; i <= Position.Item1 && x == -1; i++)
            for(int j = Position.Item2 - size + 1; j <= Position.Item2 && x == -1; j++)
                if(IsAbleToBuild(i, j, size)){
                    x = i;
                    y = j;
                }
        if(x != -1){
            TravelTarget = null;
            GameMap.AddObjectToMap(x, y, new Construction(Owner, _inConstruction, this));
        }
        else{
            Constructing = false;
            Owner.Wood += 2000;
            _inConstruction = null;
        }
    }
    private bool IsAbleToBuild(int x, int y, int size){
        //czy mieści się na mapie
        if(x < 0 || y < 0) return false;
        if(x + size > GameMap.Size || y + size > GameMap.Size) return false;

        //czy nie ma obiektów na polach na których chce budować
        for(int i = x; i < x+size; i++){
            for(int j = y; j < y+size; j++){
                if(GameMap.Map[i,j] != null && GameMap.Map[i,j] != this)
                    return false;
            }
        }

        return true;
    }
}