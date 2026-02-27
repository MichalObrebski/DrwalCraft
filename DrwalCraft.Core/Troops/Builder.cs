using DrwalCraft.Core.Buildings;

namespace DrwalCraft.Core.Troops;

public class Builder : Troop, ICanCreate{
    public List<ItemToCreate> Products {get; set;}
    public List<string> Pricing {get; set;}
    private Building? _objectInProduction;
    public bool InProduction {set; get;}
    public override (int, int) Target {
        set => TravelTarget = value;
    }
    public GameObject? ObjectInProduction {
        get => _objectInProduction;
        set{
            if(value is Building building)
                _objectInProduction = building;
        }
    }
    public int ProductionTime {get => 0;}
    public int ProductionProgress {get => 0;}

    public Builder(Player player) : base(player, "Builder.png"){
        Name = "Builder";
        MaxHp = 50;
        Hp = 50;
        InProduction = false;
        _speed = 8;
        Products = new();
        Pricing = new();

        Products.Add(new ItemToCreate(typeof(Barrack), 2000, 0));
        Pricing.Add("Barrack: 2000");
    }

    public override void MainAction(){
        if(TravelTarget != null)
            Move();
    }

    public void DoMessage(Type building)
    {
        ObjectsActions.BuildAddMessage(this.Id, _objectInProduction);
    }
    //z metodą create jest taki problem że jest wywoływana z wątku UI przez co nie jest zsynchronizowana z tickiem i moze wystąpić taki problem że zostanie wywołana podczas chodzenia co skutkuje tym że builder stoi obok podczas budowy a po zbudowaniu budynku jest w dwóch miejscach. dodatkowo zaczyna zapierdalać zamiast chodzić po zbudowaniu baraku trzeba zrobić tak żebyśmy mieli pewność że Create i MainAction będą zsynchronizowane nie tylko w builderze ale we wszystkich ICanCreate. teraz idę spać. ez
    public void Create(ItemToCreate item){
        if(InProduction) return;
        InProduction = true;

        ObjectInProduction = item.Make(Owner);

        if(_objectInProduction is null) return;

        int size = _objectInProduction.Size;
        int x = -1, y = -1;
        for(int i = Position.Item1 - size + 1; i <= Position.Item1 && x == -1; i++)
            for(int j = Position.Item2 - size + 1; j <= Position.Item2 && x == -1; j++)
                if(IsAbleToBuild(i, j, size)){
                    x = i;
                    y = j;
                }
        if(x != -1){
            TravelTarget = null;
            GameMap.AddObjectToMap(x, y, new Construction(Owner, _objectInProduction, this));
        }
        else{
            InProduction = false;
            Owner.Wood += item.PriceWood;
            _objectInProduction = null;
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