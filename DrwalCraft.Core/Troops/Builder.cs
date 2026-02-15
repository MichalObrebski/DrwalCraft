using DrwalCraft.Core.Buildings;

namespace DrwalCraft.Core.Troops;

public class Builder : Troop{
    private Player _player;
    public List<Type> Products {get; set;}
    public List<string> Pricing {get; set;}
    private Building? _inConstruction;
    public bool Constructing {set; get;}
    public Builder(Player player) : base(player, "Builder.png"){
        Name = "Builder";
        MaxHp = 50;
        Hp = 50;
        Constructing = false;
        _speed = 8;
        Products = new();
        Pricing = new();
        _player = player;
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
            if(_player.Wood >= 2000){
                _player.Wood -= 2000;
                _inConstruction = new Barrack(_player);
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
            GameMap.AddObjectToMap(x, y, new Construction(_player, _inConstruction, this));
        }
        else{
            Constructing = false;
            _player.Wood += 2000;
            _inConstruction = null;
        }
    }
    private bool IsAbleToBuild(int x, int y, int size){
        if(x < 0 || y < 0) return false;
        if(x + size > GameMap.Size || y + size > GameMap.Size) return false;
        for(int i = x; i < x+size; i++){
            for(int j = y; j < y+size; j++){
                if(GameMap.Map[i,j].GameObject != null && GameMap.Map[i,j].GameObject != this)
                    return false;
            }
        }
        return true;
    }
}